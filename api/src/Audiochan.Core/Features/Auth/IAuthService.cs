using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Auth.Dtos;
using Audiochan.Core.Features.Users.Dtos;

namespace Audiochan.Core.Features.Auth;

public interface IAuthService
{
    Task<UserDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}