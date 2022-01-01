using Audiochan.Application.Features.Users.Models;
using Audiochan.Application.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.GraphQL.Users.DataLoaders;

public class FollowingByUserIdDataLoader : GroupedDataLoader<long, UserDto>
{
    private static readonly string CacheKey = GetCacheKeyType<FollowingByUserIdDataLoader>();
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IMapper _mapper;
    
    public FollowingByUserIdDataLoader(IBatchScheduler batchScheduler, 
        IDbContextFactory<ApplicationDbContext> dbContextFactory, 
        IMapper mapper, 
        DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    protected override async Task<ILookup<long, UserDto>> LoadGroupedBatchAsync(IReadOnlyList<long> keys, 
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var list = await dbContext.Users
            .Where(u => keys.Contains(u.Id))
            .SelectMany(u => u.Followings)
            .Include(fu => fu.Target)
            .ProjectTo<FollowedUserDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        
        TryAddToCache(CacheKey, 
            list, 
            item => item.TargetId, 
            item => item.Target);

        return list.ToLookup(t => t.ObserverId, t => t.Target);
    }
}