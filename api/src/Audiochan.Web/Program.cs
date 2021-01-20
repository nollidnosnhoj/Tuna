using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Entities;
using Audiochan.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DbContext = Audiochan.Infrastructure.Data.DbContext;

namespace Audiochan.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<DbContext>();
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var roleManager = services.GetRequiredService<RoleManager<Role>>();
                    await context.Database.MigrateAsync();
                    if (!await userManager.Users.AnyAsync())
                    {
                        var superuser = new User
                        {
                            UserName = "superuser",
                            DisplayName = "Superuser",
                            Email = "superuser@localhost",
                        };

                        // TODO: Do not hardcode superuser password when deploying into production haha
                        await userManager.CreateAsync(superuser, "Password1");

                        var superUserRole = await roleManager.FindByNameAsync(UserRoleConstants.Admin);

                        if (superUserRole == null)
                        {
                            await roleManager.CreateAsync(new Role{Name = UserRoleConstants.Admin});
                        }

                        await userManager.AddToRoleAsync(superuser, UserRoleConstants.Admin);
                    }

                    if (!await context.Genres.AnyAsync())
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
                        await context.SaveChangesAsync();
                    }
                }
                catch(Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                    throw;
                }
            }
            
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
