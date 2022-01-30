using Audiochan.Application.Persistence;
using Audiochan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.GraphQL.Users.DataLoaders;

public class UserByIdDataLoader : BatchDataLoader<long, User>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public UserByIdDataLoader(IBatchScheduler batchScheduler, 
        IDbContextFactory<ApplicationDbContext> dbContextFactory, DataLoaderOptions? options = null) 
        : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task<IReadOnlyDictionary<long, User>> LoadBatchAsync(IReadOnlyList<long> keys, 
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Users
            .Where(a => keys.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, cancellationToken);
    }
}