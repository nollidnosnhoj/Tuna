using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Extensions;
using Audiochan.Core.Extensions.MappingExtensions;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models.Responses;
using Audiochan.Core.Models.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.API.Features.Followers.GetFollowings
{
    public class GetUserFollowingsRequestHandler : IRequestHandler<GetUserFollowingsRequest, PagedList<FollowingViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetUserFollowingsRequestHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedList<FollowingViewModel>> Handle(GetUserFollowingsRequest request,
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