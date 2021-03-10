using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence
{
    public static class ApplicationDbSeeder
    {
        public static async Task GetSeedAsync(ApplicationDbContext context, UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            if (!await userManager.Users.AnyAsync())
            {
                var superuser = new User("superuser", "superuser@localhost", DateTime.UtcNow);

                // TODO: Do not hardcode superuser password when deploying into production haha
                await userManager.CreateAsync(superuser, "Password1");

                var superUserRole = await roleManager.FindByNameAsync(UserRoleConstants.Admin);

                if (superUserRole == null)
                {
                    await roleManager.CreateAsync(new Role {Name = UserRoleConstants.Admin});
                }

                await userManager.AddToRoleAsync(superuser, UserRoleConstants.Admin);
            }

            await AddDefaultGenresAsync(context);
        }

        public static async Task<int> AddDefaultGenresAsync(ApplicationDbContext context)
        {
            if (!context.Genres.Any())
            {
                var genres = new List<Genre>
                {
                    new() {Name = "Alternative Rock", Slug = "alternative-rock"},
                    new() {Name = "Ambient", Slug = "ambient"},
                    new() {Name = "Classical", Slug = "classical"},
                    new() {Name = "Country", Slug = "country"},
                    new() {Name = "Deep House", Slug = "deep-house"},
                    new() {Name = "Disco", Slug = "disco"},
                    new() {Name = "Drum & Bass", Slug = "drum-n-bass"},
                    new() {Name = "Dubstep", Slug = "dubstep"},
                    new() {Name = "Electronic", Slug = "electronic"},
                    new() {Name = "Folk", Slug = "folk"},
                    new() {Name = "House", Slug = "house"},
                    new() {Name = "Indie", Slug = "indie"},
                    new() {Name = "Jazz & Blue", Slug = "jazz-n-blue"},
                    new() {Name = "Latin", Slug = "latin"},
                    new() {Name = "Metal", Slug = "metal"},
                    new() {Name = "Miscellaneous", Slug = "misc"},
                    new() {Name = "Piano", Slug = "piano"},
                    new() {Name = "Pop", Slug = "pop"},
                    new() {Name = "R&B & Soul", Slug = "rnb-n-soul"},
                    new() {Name = "Reggae", Slug = "reggae"},
                    new() {Name = "Rock", Slug = "rock"},
                    new() {Name = "Soundtrack", Slug = "soundtrack"},
                    new() {Name = "Techno", Slug = "techno"},
                    new() {Name = "Trance", Slug = "trance"},
                    new() {Name = "Trap", Slug = "trap"},
                    new() {Name = "World", Slug = "world"}
                };

                await context.Genres.AddRangeAsync(genres);
                return await context.SaveChangesAsync();
            }

            return await Task.FromResult(0);
        }
    }
}