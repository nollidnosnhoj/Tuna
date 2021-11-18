using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence
{
    public static class ApplicationDbSeeder
    {
        public static async Task SeedDefaultArtistAsync(ApplicationDbContext dbContext, IPasswordHasher passwordHasher)
        {
            var artistId = await dbContext.Artists
                .Where(u => u.UserName == "superuser")
                .Select(u => u.Id)
                .SingleOrDefaultAsync();
            
            if (artistId == default)
            {
                // TODO: Do not hardcode superuser password when deploying into production haha
                var passwordHash = passwordHasher.Hash("Password1");
                
                var superuser = new Artist("superuser", "superuser@localhost", passwordHash);

                await dbContext.Artists.AddAsync(superuser);
                await dbContext.SaveChangesAsync();
                artistId = superuser.Id;
            }

            await AudioSeedAsync(dbContext, artistId);
        }
        
        public static async Task SeedDefaultUserAsync(ApplicationDbContext dbContext, IPasswordHasher passwordHasher)
        {
            var userId = await dbContext.Users
                .Where(u => u.UserName == "defaultuser")
                .Select(u => u.Id)
                .SingleOrDefaultAsync();
            
            if (userId == default)
            {
                // TODO: Do not hardcode superuser password when deploying into production haha
                var passwordHash = passwordHasher.Hash("Password1");
                
                var user = new User("defaultuser", "default@localhost", passwordHash);

                await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task AudioSeedAsync(ApplicationDbContext dbContext, long artistId)
        {
            if (!await dbContext.Audios.AnyAsync(a => a.ArtistId == artistId))
            {
                var audios = new List<Audio>
                {
                    CreateDemoAudio(0, artistId, 157, 2200868),
                    CreateDemoAudio(1, artistId, 146, 2050193),
                    CreateDemoAudio(2, artistId, 147, 2945044),
                    CreateDemoAudio(3, artistId, 154, 2169782),
                    CreateDemoAudio(4, artistId, 169, 2370193),
                    CreateDemoAudio(5, artistId, 105, 1481873),
                    CreateDemoAudio(6, artistId, 171, 3432489),
                    CreateDemoAudio(7, artistId, 194, 2718353),
                    CreateDemoAudio(8, artistId, 230, 3224136),
                    CreateDemoAudio(9, artistId, 244, 3423450)
                };
                await dbContext.Audios.AddRangeAsync(audios);
                await dbContext.SaveChangesAsync();
            }
        }

        private static Audio CreateDemoAudio(int index, long userId, long duration, long fileSize)
        {
            return new Audio
            {
                ArtistId = userId,
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