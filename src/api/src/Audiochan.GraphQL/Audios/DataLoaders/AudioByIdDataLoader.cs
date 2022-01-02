using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.GraphQL.Audios.DataLoaders;

public class AudioByIdDataLoader : BatchDataLoader<long, AudioDto>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IMapper _mapper;

    public AudioByIdDataLoader(IBatchScheduler batchScheduler, 
        IDbContextFactory<ApplicationDbContext> dbContextFactory, 
        IMapper mapper, 
        DataLoaderOptions? options = null) : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    protected override async Task<IReadOnlyDictionary<long, AudioDto>> LoadBatchAsync(IReadOnlyList<long> keys, 
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Audios
            .Where(a => keys.Contains(a.Id))
            .ProjectTo<AudioDto>(_mapper.ConfigurationProvider)
            .ToDictionaryAsync(a => a.Id, cancellationToken);
    }
}