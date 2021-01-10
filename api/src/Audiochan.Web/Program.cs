using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Audiochan.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                    var context = services.GetRequiredService<AudiochanContext>();
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var roleManager = services.GetRequiredService<RoleManager<Role>>();
                    await context.Database.MigrateAsync();
                    if (await userManager.Users.AnyAsync())
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
                            await roleManager.CreateAsync(new Role(UserRoleConstants.Admin));
                        }

                        await userManager.AddToRoleAsync(superuser, UserRoleConstants.Admin);
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
