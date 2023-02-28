using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Users.Mappings;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Core.Persistence;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.DataLoaders;

public class GetUserDataLoader : BatchDataLoader<long, UserDto>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    public GetUserDataLoader(IBatchScheduler batchScheduler, IDbContextFactory<ApplicationDbContext> dbContextFactory, DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task<IReadOnlyDictionary<long, UserDto>> LoadBatchAsync(IReadOnlyList<long> keys, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Users
            .Where(x => keys.Contains(x.Id))
            .ProjectToDto()
            .ToDictionaryAsync(x => x.Id, cancellationToken);
    }
}