using Audiochan.Application.Features.Users.Models;
using Audiochan.Application.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.GraphQL.Users.DataLoaders;

public class UserByNameDataLoader : BatchDataLoader<string, UserDto>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IMapper _mapper;

    public UserByNameDataLoader(IBatchScheduler batchScheduler, 
        IDbContextFactory<ApplicationDbContext> dbContextFactory, IMapper mapper, DataLoaderOptions? options = null) 
        : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    protected override async Task<IReadOnlyDictionary<string, UserDto>> LoadBatchAsync(IReadOnlyList<string> keys, 
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Users
            .Where(a => keys.Contains(a.UserName))
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .ToDictionaryAsync(a => a.UserName, cancellationToken);
    }
}