using System;
using System.Threading.Tasks;
using Audiochan.Application.Persistence;
using Audiochan.Application.Services;
using Audiochan.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Audiochan.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    var env = services.GetRequiredService<IWebHostEnvironment>();
                    var context = services.GetRequiredService<ApplicationDbContext>();

                    Log.Information("Migrating database...");
                    await context.Database.MigrateAsync();

                    if (env.IsDevelopment())
                    {
                        Log.Information("Seeding users and default audios.");
                        var pwHasher = services.GetRequiredService<IPasswordHasher>();
                        var userId = await ApplicationDbSeeder.UserSeedAsync(context, pwHasher);
                        if (userId > 0)
                        {
                            await ApplicationDbSeeder.AudioSeedAsync(context, userId);
                        }
                    }
                }

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}