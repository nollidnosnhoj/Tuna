using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Persistence.Repositories
{
    public interface IAudioRepository : IEntityRepository<Audio>
    {
        Task<Audio?> LoadAudioWithFavorites(long audioId, long observerId = 0,
            CancellationToken cancellationToken = default);
    }
}