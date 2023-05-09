using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tuna.Application.Persistence;
using Tuna.Application.Persistence.Repositories;
using Tuna.Domain.Entities;

namespace Tuna.Infrastructure.Persistence.Repositories;

public class AudioRepository : EfRepository<Audio>, IAudioRepository
{
    public AudioRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Audio?> LoadAudioWithFavorites(long audioId, long observerId = 0,
        CancellationToken cancellationToken = default)
    {
        var queryable = Queryable;

        if (observerId > 0)
            queryable = queryable.Include(a =>
                a.FavoriteAudios.Where(fa => fa.UserId == observerId));
        else
            queryable = queryable.Include(a => a.FavoriteAudios);

        return await queryable.SingleOrDefaultAsync(a => a.Id == audioId, cancellationToken);
    }
}