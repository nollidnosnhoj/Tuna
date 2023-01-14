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
    public record GetUserFollowingsQuery : IHasOffsetPage, IQueryRequest<OffsetPagedListDto<FollowingViewModel>>
    {
        public string Username { get; init; } = string.Empty;
        public int Offset { get; init; }
        public int Size { get; init; }
    }

    public class GetUserFollowingsQueryHandler : IRequestHandler<GetUserFollowingsQuery, OffsetPagedListDto<FollowingViewModel>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetUserFollowingsQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OffsetPagedListDto<FollowingViewModel>> Handle(GetUserFollowingsQuery query,
            CancellationToken cancellationToken)
        {
            var list = await _dbContext.FollowedUsers
                .Where(fu => fu.Observer.UserName == query.Username)
                .OrderByDescending(fu => fu.FollowedDate)
                .ProjectToFollowing()
                .OffsetPaginateAsync(query.Offset, query.Size, cancellationToken);
            return new OffsetPagedListDto<FollowingViewModel>(list, query.Offset, query.Size);
        }
    }
}