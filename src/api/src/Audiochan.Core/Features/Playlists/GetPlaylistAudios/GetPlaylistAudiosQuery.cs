using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Playlists.GetPlaylistAudios
{
    public record GetPlaylistAudiosQuery(long Id) : IHasCursorPage<long>, IRequest<CursorPagedListDto<AudioDto>>
    {
        public long? Cursor { get; init; }
        public int Size { get; init; }
    }
    
    public class GetPlaylistAudiosQueryHandler : IRequestHandler<GetPlaylistAudiosQuery, CursorPagedListDto<AudioDto>>
    {
        private readonly ApplicationDbContext _unitOfWork;
        private readonly long _currentUserId;

        public GetPlaylistAudiosQueryHandler(ApplicationDbContext unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<CursorPagedListDto<AudioDto>> Handle(GetPlaylistAudiosQuery request, CancellationToken cancellationToken)
        {
            var playlistExists = await _unitOfWork.Playlists
                .AsNoTracking()
                .Where(p => p.Id == request.Id)
                .Where(p => p.UserId == _currentUserId || p.Visibility == Visibility.Public)
                .AnyAsync(cancellationToken);

            if (!playlistExists)
            {
                return new CursorPagedListDto<AudioDto>(new List<AudioDto>(), null, request.Size);
            }
            
            return await _unitOfWork.PlaylistAudios
                .AsNoTracking()
                .Where(pa => pa.PlaylistId == request.Id)
                .Select(pa => pa.Audio)
                .Where(a => a.UserId == _currentUserId || a.Visibility == Visibility.Public)
                .Select(AudioMaps.AudioToView(_currentUserId))
                .CursorPaginateAsync(request, cancellationToken);
        }
    }
}