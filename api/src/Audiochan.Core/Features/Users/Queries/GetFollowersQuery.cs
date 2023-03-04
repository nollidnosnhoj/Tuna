using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Shared.Mediatr;
using Audiochan.Shared.Models;
using Audiochan.Core.Features.Users.Mappings;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Core.Persistence;
using HotChocolate.Types.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.Queries;

public record GetFollowersQuery(long UserId, int? Skip, int? Take) : OffsetPagedQuery(Skip, Take), IQueryRequest<CollectionSegment<UserDto>>;

public class GetFollowersQueryHandler : IRequestHandler<GetFollowersQuery, CollectionSegment<UserDto>>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public GetFollowersQueryHandler(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    public async  Task<CollectionSegment<UserDto>> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.FollowedUsers
            .Where(x => x.TargetId == request.UserId)
            .OrderByDescending(x => x.FollowedDate)
            .Select(x => x.Observer)
            .ProjectToDto()
            .ApplyOffsetPaginationAsync(request.Skip, request.Take, cancellationToken);
    }
}