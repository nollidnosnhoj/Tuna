using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Followers.GetFollowers
{
    public record GetUserFollowersQuery : IHasPage, IRequest<PagedListDto<FollowerViewModel>>
    {
        public string Username { get; init; } = string.Empty;
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class GetUserFollowersQueryHandler : IRequestHandler<GetUserFollowersQuery, PagedListDto<FollowerViewModel>>
    {
        private readonly ApplicationDbContext _unitOfWork;

        public GetUserFollowersQueryHandler(ApplicationDbContext unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedListDto<FollowerViewModel>> Handle(GetUserFollowersQuery query,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.Users
                .AsNoTracking()
                .Include(u => u.Followers)
                .Where(u => u.UserName == query.Username)
                .SelectMany(u => u.Followers)
                .OrderByDescending(fu => fu.FollowedDate)
                .Select(FollowedUserMaps.UserToFollowerFunc)
                .PaginateAsync(query, cancellationToken);
        }
    }
}