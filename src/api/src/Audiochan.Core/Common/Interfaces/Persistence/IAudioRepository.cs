using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Audios;
using Audiochan.Core.Users;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Common.Interfaces.Persistence
{
    public interface IAudioRepository : IEntityRepository<Audio>
    {
        Task<List<AudioDto>> GetUserFavoriteAudios(GetUserFavoriteAudiosQuery query,
            CancellationToken cancellationToken = default);
    }
}