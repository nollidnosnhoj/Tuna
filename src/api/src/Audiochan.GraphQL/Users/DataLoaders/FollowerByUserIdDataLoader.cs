using Audiochan.Application.Features.Users.Models;
using Audiochan.Application.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.GraphQL.Users.DataLoaders;

public class FollowerByUserIdDataLoader : GroupedDataLoader<long, UserDto>
{
    private static readonly string FollowerCacheKey = GetCacheKeyType<FollowerByUserIdDataLoader>();
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IMapper _mapper;
    
    public FollowerByUserIdDataLoader(IBatchScheduler batchScheduler, 
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
            .SelectMany(u => u.Followers)
            .Include(fu => fu.Observer)
            .ProjectTo<FollowedUserDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        
        TryAddToCache(FollowerCacheKey, 
            list, 
            item => item.ObserverId, 
            item => item.Observer);

        return list.ToLookup(t => t.TargetId, t => t.Observer);
    }
}