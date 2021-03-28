using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Followers.GetFollowers
{
    public record GetFollowersQuery : PaginationQueryRequest<FollowerViewModel>
    {
        public string Username { get; init; }
    }
    
    public class GetFollowersQueryHandler : IRequestHandler<GetFollowersQuery, PagedList<FollowerViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetFollowersQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedList<FollowerViewModel>> Handle(GetFollowersQuery request,
            CancellationToken cancellationToken)
        {
            return await _dbContext.FollowedUsers
                .AsNoTracking()
                .Include(u => u.Target)
                .Include(u => u.Observer)
                .Where(u => u.Target.UserName == request.Username.Trim().ToLower())
                .ProjectToFollower()
                .PaginateAsync(request, cancellationToken);
        }
    }
}