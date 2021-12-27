using Audiochan.Application.Features.Users.Models;
using Audiochan.Application.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.GraphQL.Users.DataLoaders;

public class UserByIdDataLoader : BatchDataLoader<long, UserDto>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IMapper _mapper;

    public UserByIdDataLoader(IBatchScheduler batchScheduler, 
        IDbContextFactory<ApplicationDbContext> dbContextFactory, IMapper mapper, DataLoaderOptions? options = null) 
        : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    protected override async Task<IReadOnlyDictionary<long, UserDto>> LoadBatchAsync(IReadOnlyList<long> keys, 
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Users
            .Where(a => keys.Contains(a.Id))
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .ToDictionaryAsync(a => a.Id, cancellationToken);
    }
}