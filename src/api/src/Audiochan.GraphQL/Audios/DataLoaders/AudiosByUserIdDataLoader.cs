using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.GraphQL.Audios.DataLoaders;

public class AudiosByUserIdDataLoader : GroupedDataLoader<long, AudioDto>
{
    private static readonly string CacheKey = GetCacheKeyType<AudiosByUserIdDataLoader>();
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IMapper _mapper;
    
    public AudiosByUserIdDataLoader(IBatchScheduler batchScheduler, 
        IDbContextFactory<ApplicationDbContext> dbContextFactory, 
        IMapper mapper, 
        DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    protected override async Task<ILookup<long, AudioDto>> LoadGroupedBatchAsync(IReadOnlyList<long> keys, 
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var list = await dbContext.Users
            .Where(u => keys.Contains(u.Id))
            .SelectMany(u => u.Audios)
            .ProjectTo<AudioDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        
        TryAddToCache(CacheKey, list, item => item.Id, item => item);

        return list.ToLookup(t => t.UserId, t => t);
    }
}