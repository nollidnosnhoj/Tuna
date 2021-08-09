using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Persistence
{
    public static class ApplicationDbSeeder
    {
        public static async Task UserSeedAsync(ApplicationDbContext dbContext, IPasswordHasher passwordHasher)
        {
            if (!await dbContext.Users.AnyAsync())
            {
                // TODO: Do not hardcode superuser password when deploying into production haha
                var passwordHash = passwordHasher.Hash("Password1");
                
                var superuser = new User("superuser", "superuser@localhost", passwordHash);

                await dbContext.Users.AddAsync(superuser);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}