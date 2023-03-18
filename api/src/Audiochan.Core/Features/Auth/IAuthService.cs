using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Auth.Models;
using Audiochan.Core.Features.Auth.Results;

namespace Audiochan.Core.Features.Auth;

public interface IAuthService
{
    Task<AuthServiceResult<AuthServiceTokens>> LoginWithPasswordAsync(string login, string password, string? ipAddress, CancellationToken cancellationToken = default);
    Task<AuthServiceResult<AuthServiceTokens>> RefreshTokenAsync(string token,  string? ipAddress, CancellationToken cancellationToken = default);
    Task<AuthServiceResult> RevokeRefreshTokenAsync(string token, string? ipAddress, CancellationToken? cancellationToken = default);
}