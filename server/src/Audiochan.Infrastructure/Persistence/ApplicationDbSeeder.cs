using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.CreateAudio;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence
{
    public static class ApplicationDbSeeder
    {
        public static async Task UserSeedAsync(ApplicationDbContext context, UserManager<User> userManager,
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

        public static async Task AddDefaultAudioForDemo(ApplicationDbContext context, UserManager<User> userManager,
            ITagRepository tagRepository)
        {
            if (!await context.Audios.AnyAsync())
            {
                var user = await userManager.FindByNameAsync("superuser");
                var genres = await context.Genres.ToListAsync();

                var audio1 = new Audio
                {
                    UploadId = "audio1",
                    Title = "Dreams",
                    Duration = 388,
                    FileExt = ".mp3",
                    FileSize = 9335255,
                    UserId = user.Id,
                    GenreId = genres.Find(g => g.Slug == "indie")?.Id,
                    Tags = await tagRepository.CreateTags(new[] {"chillout", "lucid-dreams"})
                };
                var audio2 = new Audio
                {
                    UploadId = "audio2",
                    Title = "Heaven",
                    Duration = 194,
                    FileExt = ".mp3",
                    FileSize = 6239211,
                    UserId = user.Id,
                    GenreId = genres.Find(g => g.Slug == "electronic")?.Id,
                    Tags = await tagRepository.CreateTags(new[] {"newgrounds", "piano", "rave"})
                };
                var audio3 = new Audio
                {
                    UploadId = "audio3",
                    Title = "Life is Beautiful",
                    Duration = 45,
                    FileExt = ".mp3",
                    FileSize = 1823391,
                    UserId = user.Id,
                    GenreId = genres.Find(g => g.Slug == "electronic")?.Id,
                    Tags = await tagRepository.CreateTags(new[] {"happy", "anime", "hardcore", "nightcore"})
                };
                var audio4 = new Audio
                {
                    UploadId = "audio4",
                    Title = "Beginning of Time",
                    Duration = 164,
                    FileExt = ".mp3",
                    FileSize = 3952556,
                    UserId = user.Id,
                    GenreId = genres.Find(g => g.Slug == "techno")?.Id,
                    Tags = await tagRepository.CreateTags(new[] {"hard-dance"})
                };
                var audio5 = new Audio
                {
                    UploadId = "audio5",
                    Title = "Verity",
                    Duration = 219,
                    FileExt = ".mp3",
                    FileSize = 8788667,
                    UserId = user.Id,
                    GenreId = genres.Find(g => g.Slug == "trance")?.Id,
                    Tags = await tagRepository.CreateTags(new[] {"vocals"})
                };
                await context.Audios.AddRangeAsync(audio1, audio2, audio3, audio4, audio5);
                await context.SaveChangesAsync();
            }
        }
    }
}