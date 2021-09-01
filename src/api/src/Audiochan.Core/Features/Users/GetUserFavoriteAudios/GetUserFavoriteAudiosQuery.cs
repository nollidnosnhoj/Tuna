using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.GetUserFavoriteAudios
{
    public record GetUserFavoriteAudiosQuery : IHasOffsetPage, IRequest<OffsetPagedListDto<AudioDto>>
    {
        public string? Username { get; set; }
        public int Offset { get; init; }
        public int Size { get; init; }
    }
    
    public class GetUserFavoriteAudiosQueryHandler : IRequestHandler<GetUserFavoriteAudiosQuery, OffsetPagedListDto<AudioDto>>
    {
        private readonly ApplicationDbContext _unitOfWork;
        private readonly long _currentUserId;

        public GetUserFavoriteAudiosQueryHandler(ApplicationDbContext unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<OffsetPagedListDto<AudioDto>> Handle(GetUserFavoriteAudiosQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Users
                .AsNoTracking()
                .Where(u => u.UserName == query.Username)
                .SelectMany(u => u.FavoriteAudios)
                .OrderByDescending(a => a.Id)
                .Select(AudioMaps.AudioToView(_currentUserId))
                .OffsetPaginateAsync(query, cancellationToken);
        }
    }
}