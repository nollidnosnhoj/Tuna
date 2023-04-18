using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tuna.Infrastructure.Identity.Models;

namespace Tuna.Infrastructure.Identity;

public static class IdentityExtensions
{
    public static Task<AuthUser?> FindUserByRefreshTokenAsync(this UserManager<AuthUser> userManager, string refreshToken)
    {
        return userManager.Users
            .Include(x => x.RefreshTokens)
            .SingleOrDefaultAsync(x => x.RefreshTokens.Any(t => t.Token == refreshToken));
    }
}