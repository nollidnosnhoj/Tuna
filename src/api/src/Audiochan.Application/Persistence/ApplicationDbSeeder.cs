using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Application.Commons.Services;
using Audiochan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Application.Persistence
{
    public static class ApplicationDbSeeder
    {
        public static async Task<long> UserSeedAsync(ApplicationDbContext dbContext, IPasswordHasher passwordHasher)
        {
            var userId = await dbContext.Users
                .Where(u => u.UserName == "superuser")
                .Select(u => u.Id)
                .SingleOrDefaultAsync();
            
            if (userId == default)
            {
                // TODO: Do not hardcode superuser password when deploying into production haha
                var passwordHash = passwordHasher.Hash("Password1");
                
                var superuser = new User("superuser", "superuser@localhost", passwordHash);

                await dbContext.Users.AddAsync(superuser);
                await dbContext.SaveChangesAsync();
                userId = superuser.Id;
            }

            return userId;
        }

        public static async Task AudioSeedAsync(ApplicationDbContext dbContext, long userId)
        {
            if (!await dbContext.Audios.AnyAsync(a => a.UserId == userId))
            {
                var audios = new List<Audio>
                {
                    CreateDemoAudio(0, userId, 157, 2200868),
                    CreateDemoAudio(1, userId, 146, 2050193),
                    CreateDemoAudio(2, userId, 147, 2945044),
                    CreateDemoAudio(3, userId, 154, 2169782),
                    CreateDemoAudio(4, userId, 169, 2370193),
                    CreateDemoAudio(5, userId, 105, 1481873),
                    CreateDemoAudio(6, userId, 171, 3432489),
                    CreateDemoAudio(7, userId, 194, 2718353),
                    CreateDemoAudio(8, userId, 230, 3224136),
                    CreateDemoAudio(9, userId, 244, 3423450)
                };
                await dbContext.Audios.AddRangeAsync(audios);
                await dbContext.SaveChangesAsync();
            }
        }

        private static Audio CreateDemoAudio(int index, long userId, long duration, long fileSize)
        {
            return new Audio
            {
                UserId = userId,
                Title = $"Test0{index}",
                Description = "This audio is created by bensound.com, used for demo purposes only.",
                Duration = duration,
                Created = DateTime.UtcNow,
                File = $"test0{index}.mp3",
                Size = fileSize,
            };
        }
    }
}