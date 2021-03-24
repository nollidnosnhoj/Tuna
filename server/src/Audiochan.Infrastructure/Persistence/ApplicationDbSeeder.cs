using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence
{
    public static class ApplicationDbSeeder
    {
        public static async Task UserSeedAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
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

        }
        
        public static async Task AddDefaultAudioForDemo(ApplicationDbContext context, UserManager<User> userManager,
            ITagRepository tagRepository)
        {
            if (!await context.Audios.AnyAsync())
            {
                var user = await userManager.FindByNameAsync("superuser");

                var audio1 = new Audio
                {
                    UploadId = "audio1",
                    Title = "Dreams",
                    Duration = 388,
                    FileExt = ".mp3",
                    FileSize = 9335255,
                    UserId = user.Id,
                    Tags = await tagRepository.CreateTags(new[] {"chillout", "lucid-dreams"}),
                    Visibility = Visibility.Public
                };
                var audio2 = new Audio
                {
                    UploadId = "audio2",
                    Title = "Heaven",
                    Duration = 194,
                    FileExt = ".mp3",
                    FileSize = 6239211,
                    UserId = user.Id,
                    Tags = await tagRepository.CreateTags(new[] {"newgrounds", "piano", "rave"}),
                    Visibility = Visibility.Public
                };
                var audio3 = new Audio
                {
                    UploadId = "audio3",
                    Title = "Life is Beautiful",
                    Duration = 45,
                    FileExt = ".mp3",
                    FileSize = 1823391,
                    UserId = user.Id,
                    Tags = await tagRepository.CreateTags(new[] {"happy", "anime", "hardcore", "nightcore"}),
                    Visibility = Visibility.Public
                };
                var audio4 = new Audio
                {
                    UploadId = "audio4",
                    Title = "Beginning of Time",
                    Duration = 164,
                    FileExt = ".mp3",
                    FileSize = 3952556,
                    UserId = user.Id,
                    Tags = await tagRepository.CreateTags(new[] {"hard-dance"}),
                    Visibility = Visibility.Unlisted
                };
                var audio5 = new Audio
                {
                    UploadId = "audio5",
                    Title = "Verity",
                    Duration = 219,
                    FileExt = ".mp3",
                    FileSize = 8788667,
                    UserId = user.Id,
                    Tags = await tagRepository.CreateTags(new[] {"vocals"}),
                    Visibility = Visibility.Unlisted
                };
                await context.Audios.AddRangeAsync(audio1, audio2, audio3, audio4, audio5);
                await context.SaveChangesAsync();
            }
        }
    }
}