using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Common.Models;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Core.Features.Users.Mappings;
using Audiochan.Core.Persistence;
using HotChocolate.Types.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users;

public record GetFollowingsQuery(long UserId, int? Skip, int? Take) : OffsetPagedQuery(Skip, Take), IQueryRequest<CollectionSegment<UserViewModel>>;

public class GetFollowingsQueryHandler : IRequestHandler<GetFollowingsQuery, CollectionSegment<UserViewModel>>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public GetFollowingsQueryHandler(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    public async  Task<CollectionSegment<UserViewModel>> Handle(GetFollowingsQuery request, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.FollowedUsers
            .Where(x => x.ObserverId == request.UserId)
            .OrderByDescending(x => x.FollowedDate)
            .Select(x => x.Target)
            .ProjectToUser()
            .ApplyOffsetPaginationAsync(request.Skip, request.Take, cancellationToken);
    }
}