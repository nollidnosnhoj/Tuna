using System.Threading;
using System.Threading.Tasks;
using Tuna.Application.Entities;

namespace Tuna.Application.Persistence.Repositories
{
    public interface IAudioRepository : IEntityRepository<Audio>
    {
        Task<Audio?> LoadAudioWithFavorites(long audioId, long observerId = 0,
            CancellationToken cancellationToken = default);
    }
}