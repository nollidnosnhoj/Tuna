using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Features.Users.Models;

namespace Audiochan.Application.Services;

public interface IAuthService
{
    Task LoginAsync(UserDto user, CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);
}