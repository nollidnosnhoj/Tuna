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

public record GetFollowersQuery(long UserId, int? Skip, int? Take) : OffsetPagedQuery(Skip, Take), IQueryRequest<CollectionSegment<UserViewModel>>;

public class GetFollowersQueryHandler : IRequestHandler<GetFollowersQuery, CollectionSegment<UserViewModel>>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public GetFollowersQueryHandler(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    public async  Task<CollectionSegment<UserViewModel>> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.FollowedUsers
            .Where(x => x.TargetId == request.UserId)
            .OrderByDescending(x => x.FollowedDate)
            .Select(x => x.Observer)
            .ProjectToUser()
            .ApplyOffsetPaginationAsync(request.Skip, request.Take, cancellationToken);
    }
}