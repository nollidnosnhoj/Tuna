using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.GetUserAudios
{
    public class GetUsersAudioQuery : IHasPage, IRequest<PagedListDto<AudioViewModel>>
    {
        public string? Username { get; set; }
        public int Page { get; init; } = 1;
        public int Size { get; init; } = 30;
    }

    public class GetUsersAudioQueryHandler : IRequestHandler<GetUsersAudioQuery, PagedListDto<AudioViewModel>>
    {
        private readonly ApplicationDbContext _unitOfWork;
        private readonly long _currentUserId;

        public GetUsersAudioQueryHandler(ApplicationDbContext unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<PagedListDto<AudioViewModel>> Handle(GetUsersAudioQuery request,
            CancellationToken cancellationToken)
        {
            IQueryable<Audio> queryable = _unitOfWork.Users
                .AsNoTracking()
                .Include(u => u.Audios)
                .Where(u => request.Username == u.UserName.ToLower())
                .SelectMany(a => a.Audios)
                .Include(a => a.Tags)
                .Include(a => a.User);

            queryable = UserHelpers.IsValidId(_currentUserId)
                ? queryable.FilterVisibility(_currentUserId, FilterVisibilityMode.OnlyPublic)
                : queryable.Where(a => a.Visibility == Visibility.Public);

            return await queryable
                .Select(AudioMaps.AudioToView)
                .PaginateAsync(request, cancellationToken);
        }
    }
}