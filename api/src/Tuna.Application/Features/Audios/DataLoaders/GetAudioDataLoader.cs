using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuna.Application.Features.Audios.Mappings;
using GreenDonut;
using Microsoft.EntityFrameworkCore;
using Tuna.Application.Features.Audios.Models;
using Tuna.Application.Persistence;

namespace Tuna.Application.Features.Audios.DataLoaders;

public class GetAudioDataLoader : BatchDataLoader<long, AudioDto>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    public GetAudioDataLoader(IBatchScheduler batchScheduler, IDbContextFactory<ApplicationDbContext> dbContextFactory, DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task<IReadOnlyDictionary<long, AudioDto>> LoadBatchAsync(IReadOnlyList<long> keys, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Audios
            .Where(x => keys.Contains(x.Id))
            .ProjectToDto()
            .ToDictionaryAsync(x => x.Id, cancellationToken);
    }
}