using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.GraphQL.Audios.DataLoaders;

public class FavoriteAudiosByUserIdDataLoader : GroupedDataLoader<long, AudioDto>
{
    private static readonly string FollowerCacheKey = GetCacheKeyType<FavoriteAudiosByUserIdDataLoader>();
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IMapper _mapper;
    
    public FavoriteAudiosByUserIdDataLoader(IBatchScheduler batchScheduler, 
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
            .SelectMany(u => u.FavoriteAudios)
            .Include(fu => fu.Audio)
            .ProjectTo<FavoriteAudioDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        
        TryAddToCache(FollowerCacheKey, 
            list, 
            item => item.AudioId, 
            item => item.Audio);

        return list.ToLookup(t => t.UserId, t => t.Audio);
    }
}