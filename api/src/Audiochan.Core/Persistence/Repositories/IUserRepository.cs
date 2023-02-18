using System.Threading;
using System.Threading.Tasks;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Persistence.Repositories
{
    public interface IUserRepository : IEntityRepository<User>
    {
        Task<User?> GetUserWithLogin(string login, CancellationToken cancellationToken = default);
        Task<User?> LoadUserWithFollowers(long targetId, long observerId, CancellationToken cancellationToken = default);
        Task<bool> CheckIfUsernameExists(string userName, CancellationToken cancellationToken = default);
        Task<bool> CheckIfEmailExists(string email, CancellationToken cancellationToken = default);
    }
}