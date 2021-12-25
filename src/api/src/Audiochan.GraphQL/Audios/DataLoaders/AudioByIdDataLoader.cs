using Audiochan.Application.Persistence;
using Audiochan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.GraphQL.Audios.DataLoaders;

public class AudioByIdDataLoader : BatchDataLoader<long, Audio>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public AudioByIdDataLoader(IBatchScheduler batchScheduler, 
        IDbContextFactory<ApplicationDbContext> dbContextFactory, DataLoaderOptions? options = null) 
        : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task<IReadOnlyDictionary<long, Audio>> LoadBatchAsync(IReadOnlyList<long> keys, 
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Audios
            .Where(a => keys.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, cancellationToken);
    }
}