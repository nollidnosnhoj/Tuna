using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Users.Dtos;
using Audiochan.Core.Features.Users.Mappings;
using Audiochan.Core.Persistence;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users;

public class UserQueryService : IAsyncDisposable
{
    private readonly ApplicationDbContext _dbContext;

    public UserQueryService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContext = dbContextFactory.CreateDbContext();
    }

    public async Task<UserDto?> GetUserAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(x => x.Id == userId)
            .ProjectToUser()
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<CollectionSegment<UserDto>> GetFollowings(
        long userId, 
        int? skip, 
        int? take,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.FollowedUsers
            .Where(x => x.ObserverId == userId)
            .OrderByDescending(x => x.FollowedDate)
            .Select(x => x.Target)
            .ProjectToUser()
            .ApplyOffsetPaginationAsync(skip, take, cancellationToken);
    }

    public async Task<CollectionSegment<UserDto>> GetFollowers(
        long userId, 
        int? skip, 
        int? take,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.FollowedUsers
            .Where(x => x.TargetId == userId)
            .OrderByDescending(x => x.FollowedDate)
            .Select(x => x.Observer)
            .ProjectToUser()
            .ApplyOffsetPaginationAsync(skip, take, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return _dbContext.DisposeAsync();
    }
}