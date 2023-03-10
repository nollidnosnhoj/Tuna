using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Auth.Models;
using Audiochan.Core.Features.Auth.Results;

namespace Audiochan.Core.Features.Auth;

public interface IAuthService
{
    Task<LoginResult> LoginWithPasswordAsync(string login, string password, CancellationToken cancellationToken = default);
}