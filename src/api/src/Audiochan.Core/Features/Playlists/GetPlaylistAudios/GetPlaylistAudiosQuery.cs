using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Playlists.GetPlaylistAudios
{
    public record GetPlaylistAudiosQuery(long Id) : IHasPage, IRequest<PagedListDto<AudioViewModel>>
    {
        public int Page { get; init; }
        public int Size { get; init; }
    }
    
    public class GetPlaylistAudiosQueryHandler : IRequestHandler<GetPlaylistAudiosQuery, PagedListDto<AudioViewModel>>
    {
        private readonly ApplicationDbContext _unitOfWork;
        private readonly long _currentUserId;

        public GetPlaylistAudiosQueryHandler(ApplicationDbContext unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<PagedListDto<AudioViewModel>> Handle(GetPlaylistAudiosQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Playlists
                .Include(p => p.Audios)
                .Where(p => p.Id == request.Id)
                .FilterVisibility(_currentUserId, FilterVisibilityMode.Unlisted)
                .Where(p => p.UserId == _currentUserId || p.Visibility != Visibility.Private)
                .SelectMany(p => p.Audios)
                .Select(pa => pa.Audio)
                .FilterVisibility(_currentUserId, FilterVisibilityMode.OnlyPublic)
                .Select(AudioMaps.AudioToView)
                .PaginateAsync(request, cancellationToken);
        }
    }
}