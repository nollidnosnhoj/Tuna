using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Application.Services;
using Audiochan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.Application.Persistence
{
    public static class ApplicationDbSeeder
    {
        public static async Task SeedDataAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var dateTime = serviceProvider.GetRequiredService<IDateTimeProvider>();
            var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher>();

            var superUser = await CreateDemoUserAsync(
                "superuser", 
                "superuser@localhost", 
                "Password1", 
                dbContext,
                passwordHasher);

            var testUser1 = await CreateDemoUserAsync(
                "testuser1",
                "testuser1@localhost",
                "TestPassword123",
                dbContext,
                passwordHasher);
            
            var testUser2 = await CreateDemoUserAsync(
                "testuser2",
                "testuser2@localhost",
                "TestPassword123",
                dbContext,
                passwordHasher);

            await AudioSeedAsync(superUser, dbContext, 
                CreateDemoAudio(0, superUser, 157, 2200868),
                CreateDemoAudio(1, superUser, 146, 2050193),
                CreateDemoAudio(2, superUser, 147, 2945044),
                CreateDemoAudio(3, superUser, 154, 2169782));

            await AudioSeedAsync(testUser1, dbContext,
                CreateDemoAudio(4, testUser1, 169, 2370193),
                CreateDemoAudio(5, testUser1, 105, 1481873),
                CreateDemoAudio(6, testUser1, 171, 3432489));
            
            await AudioSeedAsync(testUser2, dbContext,
                CreateDemoAudio(7, testUser2, 194, 2718353),
                CreateDemoAudio(8, testUser2, 230, 3224136),
                CreateDemoAudio(9, testUser2, 244, 3423450));

            await CreateFollowing(superUser, testUser1, dbContext, dateTime);
            await CreateFollowing(superUser, testUser2, dbContext, dateTime);

            
            await CreateAudioFavorite(GetRandomAudio(superUser.Audios), testUser1, dbContext, dateTime);
            await CreateAudioFavorite(GetRandomAudio(superUser.Audios), testUser2, dbContext, dateTime);
        }

        private static async Task<User> CreateDemoUserAsync(string userName, string email, string password, ApplicationDbContext dbContext, IPasswordHasher passwordHasher)
        {
            var user = await dbContext.Users
                .Include(u => u.Audios)
                .Include(u => u.FavoriteAudios)
                .Include(u => u.Followers)
                .Include(u => u.Followings)
                .Where(u => u.UserName == userName)
                .SingleOrDefaultAsync();

            if (user is not null) return user;
            
            var passwordHash = passwordHasher.Hash(password);
                
            var superuser = new User(userName, email, passwordHash);
            await dbContext.Users.AddAsync(superuser);
            await dbContext.SaveChangesAsync();
            return superuser;
        }

        private static async Task AudioSeedAsync(User user, ApplicationDbContext dbContext, params Audio[] audios)
        {
            if (!await dbContext.Audios.AnyAsync(a => a.UserId == user.Id))
            {
                foreach (var audio in audios)
                {
                    user.Audios.Add(audio);
                }

                await dbContext.SaveChangesAsync();
            }
        }

        private static Audio CreateDemoAudio(int index, User user, long duration, long fileSize)
        {
            return new Audio
            {
                User = user,
                Title = $"Test0{index}",
                Description = "This audio is created by bensound.com, used for demo purposes only.",
                Duration = duration,
                Created = DateTime.UtcNow,
                File = $"test0{index}.mp3",
                Size = fileSize,
            };
        }

        private static async Task CreateFollowing(User target, User observer, ApplicationDbContext dbContext, IDateTimeProvider dateTime)
        {
            if (await dbContext.FollowedUsers
                    .AnyAsync(fu => fu.ObserverId == observer.Id && fu.TargetId == target.Id))
            {
                return;
            }
            
            target.Follow(observer.Id, dateTime.Now);

            await dbContext.SaveChangesAsync();
        }

        private static async Task CreateAudioFavorite(Audio audio, User observer, ApplicationDbContext dbContext,
            IDateTimeProvider dateTime)
        {
            if (await dbContext.FavoriteAudios.AnyAsync(fa => fa.AudioId == audio.Id && fa.UserId == observer.Id))
                return;
            
            audio.Favorite(observer.Id, dateTime.Now);
            
            await dbContext.SaveChangesAsync();
        }

        private static Audio GetRandomAudio(ICollection<Audio> audios)
        {
            var rand = new Random();
            var skip = rand.Next(0, audios.Count);
            return audios.Skip(skip).First();
        }
    }
}