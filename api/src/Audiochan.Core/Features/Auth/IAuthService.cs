using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Auth.Models;

namespace Audiochan.Core.Features.Auth;

public interface IAuthService
{
    Task<LoginResult> LoginWithPasswordAsync(string login, string password, CancellationToken cancellationToken = default);
}