using System;
using System.Threading.Tasks;
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
                    var dateTime = services.GetRequiredService<IDateTimeService>();
                    await context.Database.MigrateAsync();
                    if (await userManager.Users.CountAsync() == 0)
                    {
                        var superuser = new User
                        {
                            UserName = "superuser",
                            DisplayName = "Superuser",
                            Email = "superuser@localhost",
                            Profile = new Profile(),
                            Created = dateTime.Now
                        };

                        await userManager.CreateAsync(superuser, "Password1");

                        var superUserRole = await roleManager.FindByNameAsync("Admin");

                        if (superUserRole == null)
                        {
                            superUserRole = new Role {Name = "Admin"};
                            await roleManager.CreateAsync(superUserRole);
                        }

                        await userManager.AddToRoleAsync(superuser, "Admin");
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
