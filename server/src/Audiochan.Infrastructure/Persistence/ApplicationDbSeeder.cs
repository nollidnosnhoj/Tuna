using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Builders;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Entities;
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

                var audio1 = await new AudioBuilder()
                    .AddTitle("Dreams")
                    .AddFileName("Dreams.mp3")
                    .AddFileSize(9335255)
                    .AddDuration(388)
                    .AddUserId(user.Id)
                    .AddTags(await tagRepository.GetListAsync(new[] {"chillout", "lucid-dreams"}))
                    .SetPublic(true)
                    .SetPublish(DateTime.UtcNow)
                    .BuildAsync();
                
                var audio2 = await new AudioBuilder()
                    .AddTitle("Heaven")
                    .AddFileName("Heaven.mp3")
                    .AddFileSize(6239211)
                    .AddDuration(194)
                    .AddUserId(user.Id)
                    .AddTags(await tagRepository.GetListAsync(new[] {"newgrounds", "piano", "rave"}))
                    .SetPublic(true)
                    .SetPublish(DateTime.UtcNow)
                    .BuildAsync();
                
                var audio3 = await new AudioBuilder()
                    .AddTitle("Life Is Beautiful")
                    .AddFileName("LifeIsBeautiful.mp3")
                    .AddFileSize(1823391)
                    .AddDuration(45)
                    .AddUserId(user.Id)
                    .AddTags(await tagRepository.GetListAsync(new[] {"happy", "anime", "hardcore", "nightcore"}))
                    .SetPublic(true)
                    .SetPublish(DateTime.UtcNow)
                    .BuildAsync();
                
                var audio4 = await new AudioBuilder()
                    .AddTitle("Beginning of Time")
                    .AddFileName("BeginningOfTime.mp3")
                    .AddFileSize(3952556)
                    .AddDuration(164)
                    .AddUserId(user.Id)
                    .AddTags(await tagRepository.GetListAsync(new[] {"hard-dance"}))
                    .SetPublic(false)
                    .SetPublish(DateTime.UtcNow)
                    .BuildAsync();
                
                var audio5 = await new AudioBuilder()
                    .AddTitle("Verity")
                    .AddFileName("Verity.mp3")
                    .AddFileSize(8788667)
                    .AddDuration(219)
                    .AddUserId(user.Id)
                    .AddTags(await tagRepository.GetListAsync(new[] {"vocals"}))
                    .SetPublic(false)
                    .SetPublish(DateTime.UtcNow)
                    .BuildAsync();

                audio1.FileName = "audio1.mp3";
                audio2.FileName = "audio2.mp3";
                audio3.FileName = "audio3.mp3";
                audio4.FileName = "audio4.mp3";
                audio5.FileName = "audio5.mp3";

                await context.Audios.AddRangeAsync(audio1, audio2, audio3, audio4, audio5);
                await context.SaveChangesAsync();
            }
        }
    }
}