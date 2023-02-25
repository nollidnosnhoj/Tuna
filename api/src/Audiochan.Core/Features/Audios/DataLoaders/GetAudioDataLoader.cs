using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Audios.Mappings;
using Audiochan.Core.Persistence;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.DataLoaders;

public class GetAudioDataLoader : BatchDataLoader<long, AudioViewModel>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    public GetAudioDataLoader(IBatchScheduler batchScheduler, IDbContextFactory<ApplicationDbContext> dbContextFactory, DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task<IReadOnlyDictionary<long, AudioViewModel>> LoadBatchAsync(IReadOnlyList<long> keys, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Audios
            .Where(x => keys.Contains(x.Id))
            .Project()
            .ToDictionaryAsync(x => x.Id, cancellationToken);
    }
}