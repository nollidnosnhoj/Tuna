using System.Linq;
using System.Threading.Tasks;
using Audiochan.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Identity;

public static class IdentityExtensions
{
    public static Task<AudiochanIdentityUser?> FindUserByRefreshTokenAsync(this UserManager<AudiochanIdentityUser> userManager, string refreshToken)
    {
        return userManager.Users
            .Include(x => x.RefreshTokens)
            .SingleOrDefaultAsync(x => x.RefreshTokens.Any(t => t.Token == refreshToken));
    }
}