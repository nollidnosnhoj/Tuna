using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Audios.Dtos;
using Audiochan.Core.Features.Audios.Mappings;
using Audiochan.Core.Persistence;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios;

public class AudioQueryService : IAsyncDisposable
{
    private readonly ApplicationDbContext _dbContext;

    public AudioQueryService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContext = dbContextFactory.CreateDbContext();
    }

    public async Task<AudioDto?> GetAudioAsync(long audioId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Audios
            .Where(x => x.Id == audioId)
            .Project()
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<CollectionSegment<AudioDto>> GetUserAudiosAsync(long userId, int? skip, int? take, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Audios
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Project()
            .ApplyOffsetPaginationAsync(skip, take, cancellationToken);
    }
    
    public async Task<CollectionSegment<AudioDto>> GetUserFavoriteAudiosAsync(long userId, int? skip, int? take, CancellationToken cancellationToken = default)
    {
        return await _dbContext.FavoriteAudios
            .Where(x => x.UserId == userId)
            .Select(x => x.Audio)
            .OrderByDescending(x => x.CreatedAt)
            .Project()
            .ApplyOffsetPaginationAsync(skip, take, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return _dbContext.DisposeAsync();
    }
}