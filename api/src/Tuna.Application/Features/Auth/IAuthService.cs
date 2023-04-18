using System.Threading;
using System.Threading.Tasks;
using Tuna.Application.Features.Auth.Models;
using Tuna.Application.Features.Auth.Results;

namespace Tuna.Application.Features.Auth;

public interface IAuthService
{
    Task<AuthServiceResult<AuthServiceTokens>> LoginWithPasswordAsync(string login, string password, string? ipAddress, CancellationToken cancellationToken = default);
    Task<AuthServiceResult<AuthServiceTokens>> RefreshTokenAsync(string token,  string? ipAddress, CancellationToken cancellationToken = default);
    Task<AuthServiceResult> RevokeRefreshTokenAsync(string token, string? ipAddress, CancellationToken? cancellationToken = default);
}