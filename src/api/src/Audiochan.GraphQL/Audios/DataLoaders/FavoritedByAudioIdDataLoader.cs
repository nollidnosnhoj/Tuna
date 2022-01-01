using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Application.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.GraphQL.Audios.DataLoaders;

public class FavoritedByAudioIdDataLoader : GroupedDataLoader<long, UserDto>
{
    private static readonly string CacheKey = GetCacheKeyType<FavoritedByAudioIdDataLoader>();
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IMapper _mapper;
    
    public FavoritedByAudioIdDataLoader(IBatchScheduler batchScheduler, 
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
        
        var list = await dbContext.Audios
            .Where(u => keys.Contains(u.Id))
            .SelectMany(u => u.FavoriteAudios)
            .Include(fu => fu.User)
            .ProjectTo<FavoriteAudioDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        
        TryAddToCache(CacheKey, 
            list, 
            item => item.UserId, 
            item => item.User);

        return list.ToLookup(t => t.AudioId, t => t.User);
    }
}