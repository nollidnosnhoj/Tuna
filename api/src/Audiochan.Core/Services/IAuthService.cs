using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Dtos;

namespace Audiochan.Core.Services;

public interface IAuthService
{
    Task<LoginResult> LoginWithPasswordAsync(string login, string password, CancellationToken cancellationToken = default);
}