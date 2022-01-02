using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Persistence;
using Audiochan.Application.Persistence.Repositories;
using Audiochan.Domain.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class AudioRepository : EfRepository<Audio>, IAudioRepository
    {
        public AudioRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<Audio?> LoadAudioWithFavorites(long audioId, long observerId = 0, 
            CancellationToken cancellationToken = default)
        {
            IQueryable<Audio> queryable = Queryable;

            if (observerId > 0)
            {
                queryable = queryable.Include(a => 
                    a.FavoriteAudios.Where(fa => fa.UserId == observerId));
            }
            else
            {
                queryable = queryable.Include(a => a.FavoriteAudios);
            }

            return await queryable.SingleOrDefaultAsync(a => a.Id == audioId, cancellationToken);
        }
    }
}