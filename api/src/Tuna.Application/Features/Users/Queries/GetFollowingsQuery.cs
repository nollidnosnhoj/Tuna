using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuna.Shared.Mediatr;
using Tuna.Shared.Models;
using Tuna.Application.Features.Users.Mappings;
using HotChocolate.Types.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tuna.Application.Features.Users.Models;
using Tuna.Application.Persistence;

namespace Tuna.Application.Features.Users.Queries;

public record GetFollowingsQuery(long UserId, int? Skip, int? Take) : OffsetPagedQuery(Skip, Take), IQueryRequest<CollectionSegment<UserDto>>;

public class GetFollowingsQueryHandler : IRequestHandler<GetFollowingsQuery, CollectionSegment<UserDto>>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public GetFollowingsQueryHandler(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    public async  Task<CollectionSegment<UserDto>> Handle(GetFollowingsQuery request, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.FollowedUsers
            .Where(x => x.ObserverId == request.UserId)
            .OrderByDescending(x => x.FollowedDate)
            .Select(x => x.Target)
            .ProjectToDto()
            .ApplyOffsetPaginationAsync(request.Skip, request.Take, cancellationToken);
    }
}