using System.Threading;
using System.Threading.Tasks;
using Audiochan.Domain.Entities;

namespace Audiochan.Application.Persistence.Repositories
{
    public interface IUserRepository : IEntityRepository<User>
    {
        Task<User?> LoadUserWithFollowers(long targetId, long observerId, CancellationToken cancellationToken = default);
    }
}