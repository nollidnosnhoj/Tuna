using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Tuna.Application.Features.Auth.Models;

namespace Tuna.Application.Services;

public interface IIdentityService
{
    Task<IdentityUserDto?> GetUserAsync(string id, CancellationToken cancellationToken = default);
    Task<NewIdentityUserResult> CreateUserAsync(string userName, string email, string password, CancellationToken cancellationToken = default);
    Task<IdentityResult> UpdateUserNameAsync(string identityId, string userName, CancellationToken cancellationToken = default);
    Task<IdentityResult> UpdateEmailAsync(string identityId, string email, CancellationToken cancellationToken = default);
    Task<IdentityResult> UpdatePasswordAsync(string identityId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
}