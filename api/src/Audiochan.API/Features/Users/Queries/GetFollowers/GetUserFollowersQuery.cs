using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Users.Mappings;
using Audiochan.Core.CQRS;
using Audiochan.Core.Dtos.Wrappers;
using Audiochan.Core.Extensions;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;

namespace Audiochan.Core.Users.Queries
{
    public record GetUserFollowersQuery(string Username) : IHasOffsetPage, IQueryRequest<OffsetPagedListDto<FollowerViewModel>>
    {
        public int Offset { get; init; }
        public int Size { get; init; }
    }

    public class GetUserFollowersQueryHandler : IRequestHandler<GetUserFollowersQuery, OffsetPagedListDto<FollowerViewModel>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetUserFollowersQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OffsetPagedListDto<FollowerViewModel>> Handle(GetUserFollowersQuery query,
            CancellationToken cancellationToken)
        {
            var list = await _dbContext.FollowedUsers
                .Where(fu => fu.Target.UserName == query.Username)
                .OrderByDescending(fu => fu.FollowedDate)
                .ProjectToFollower()
                .OffsetPaginateAsync(query.Offset, query.Size, cancellationToken);
            return new OffsetPagedListDto<FollowerViewModel>(list, query.Offset, query.Size);
        }
    }
}