using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.GetUserFavoriteAudios
{
    public record GetUserFavoriteAudiosQuery : IHasPage, IRequest<PagedListDto<AudioViewModel>>
    {
        public string? Username { get; set; }
        public int Page { get; init; } = 1;
        public int Size { get; init; } = 30;
    }
    
    public class GetUserFavoriteAudiosQueryHandler : IRequestHandler<GetUserFavoriteAudiosQuery, PagedListDto<AudioViewModel>>
    {
        private readonly ApplicationDbContext _unitOfWork;
        private readonly string _currentUserId;

        public GetUserFavoriteAudiosQueryHandler(ApplicationDbContext unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<PagedListDto<AudioViewModel>> Handle(GetUserFavoriteAudiosQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Users
                .AsNoTracking()
                .Include(u => u.FavoriteAudios)
                .ThenInclude(fa => fa.Audio)
                .Where(u => u.UserName == query.Username)
                .SelectMany(u => u.FavoriteAudios)
                .Select(fa => fa.Audio)
                .FilterVisibility(_currentUserId, FilterVisibilityMode.OnlyPublic)
                .Select(AudioMaps.AudioToView)
                .PaginateAsync(query, cancellationToken);
        }
    }
}