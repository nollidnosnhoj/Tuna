using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Followers.GetFollowings
{
    public record GetUserFollowingsQuery : IHasPage, IRequest<PagedListDto<FollowingViewModel>>
    {
        public string Username { get; init; } = string.Empty;
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class GetUserFollowingsQueryHandler : IRequestHandler<GetUserFollowingsQuery, PagedListDto<FollowingViewModel>>
    {
        private readonly ApplicationDbContext _unitOfWork;

        public GetUserFollowingsQueryHandler(ApplicationDbContext unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedListDto<FollowingViewModel>> Handle(GetUserFollowingsQuery query,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.Users
                .AsNoTracking()
                .Include(u => u.Followings)
                .Where(u => u.UserName == query.Username)
                .SelectMany(u => u.Followings)
                .OrderByDescending(fu => fu.FollowedDate)
                .Select(FollowedUserMaps.UserToFollowingFunc)
                .PaginateAsync(query, cancellationToken);
        }
    }
}