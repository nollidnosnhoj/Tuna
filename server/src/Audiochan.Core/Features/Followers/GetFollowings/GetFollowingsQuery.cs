using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Followers.GetFollowings
{
    public record GetFollowingsQuery : PaginationQueryRequest<FollowingViewModel>
    {
        public string Username { get; init; }
    }
    public class GetFollowingsQueryHandler : IRequestHandler<GetFollowingsQuery, PagedList<FollowingViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetFollowingsQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedList<FollowingViewModel>> Handle(GetFollowingsQuery request,
            CancellationToken cancellationToken)
        {
            return await _dbContext.FollowedUsers
                .AsNoTracking()
                .Include(u => u.Target)
                .Include(u => u.Observer)
                .Where(u => u.Observer.UserName == request.Username.Trim().ToLower())
                .ProjectToFollowing()
                .PaginateAsync(request, cancellationToken);
        }
    }
}