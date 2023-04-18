using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using Microsoft.EntityFrameworkCore;
using Tuna.Application.Features.Users.Mappings;
using Tuna.Application.Features.Users.Models;
using Tuna.Application.Persistence;

namespace Tuna.Application.Features.Users.DataLoaders;

public class GetUserDataLoader : BatchDataLoader<long, UserDto>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public GetUserDataLoader(IBatchScheduler batchScheduler, IDbContextFactory<ApplicationDbContext> dbContextFactory,
        DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task<IReadOnlyDictionary<long, UserDto>> LoadBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Users
            .Where(x => keys.Contains(x.Id))
            .ProjectToDto()
            .ToDictionaryAsync(x => x.Id, cancellationToken);
    }
}