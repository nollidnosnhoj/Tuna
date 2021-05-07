using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Builders;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Audiochan.Infrastructure.Persistence
{
    public static class ApplicationDbSeeder
    {
        public static async Task UserSeedAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if (!await userManager.Users.AnyAsync())
            {
                var superuser = new User("superuser", "superuser@localhost", Instant.FromDateTimeUtc(DateTime.Now.ToUniversalTime()));

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

                var audio1 = await new AudioBuilder()
                    .AddTitle("Dreams")
                    .AddUploadId("audio1")
                    .AddFileName("Dreams.mp3")
                    .AddFileSize(9335255)
                    .AddDuration(388)
                    .AddUser(user)
                    .AddTags(await tagRepository.GetListAsync(new[] {"chillout", "lucid-dreams"}))
                    .SetVisibility(Visibility.Public)
                    .BuildAsync();
                
                var audio2 = await new AudioBuilder()
                    .AddTitle("Heaven")
                    .AddUploadId("audio2")
                    .AddFileName("Heaven.mp3")
                    .AddFileSize(6239211)
                    .AddDuration(194)
                    .AddUser(user)
                    .AddTags(await tagRepository.GetListAsync(new[] {"newgrounds", "piano", "rave"}))
                    .SetVisibility(Visibility.Public)
                    .BuildAsync();
                
                var audio3 = await new AudioBuilder()
                    .AddTitle("Life Is Beautiful")
                    .AddUploadId("audio3")
                    .AddFileName("LifeIsBeautiful.mp3")
                    .AddFileSize(1823391)
                    .AddDuration(45)
                    .AddUser(user)
                    .AddTags(await tagRepository.GetListAsync(new[] {"happy", "anime", "hardcore", "nightcore"}))
                    .SetVisibility(Visibility.Public)
                    .BuildAsync();
                
                var audio4 = await new AudioBuilder()
                    .AddTitle("Beginning of Time")
                    .AddUploadId("audio4")
                    .AddFileName("BeginningOfTime.mp3")
                    .AddFileSize(3952556)
                    .AddDuration(164)
                    .AddUser(user)
                    .AddTags(await tagRepository.GetListAsync(new[] {"hard-dance"}))
                    .SetVisibility(Visibility.Unlisted)
                    .BuildAsync();
                
                var audio5 = await new AudioBuilder()
                    .AddTitle("Verity")
                    .AddUploadId("audio5")
                    .AddFileName("Verity.mp3")
                    .AddFileSize(8788667)
                    .AddDuration(219)
                    .AddUser(user)
                    .AddTags(await tagRepository.GetListAsync(new[] {"vocals"}))
                    .SetVisibility(Visibility.Private)
                    .BuildAsync();

                await context.Audios.AddRangeAsync(audio1, audio2, audio3, audio4, audio5);
                await context.SaveChangesAsync();
            }
        }
    }
}