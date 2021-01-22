using Audiochan.Core.Interfaces;
using Audiochan.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Audiochan.Web.Configurations
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, 
            IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                if (environment.IsDevelopment())
                    options.UseSqlite(configuration.GetConnectionString("SQLite"));
                else
                    options.UseNpgsql(configuration.GetConnectionString("PostgreSQL"));
                
                options
                    .EnableSensitiveDataLogging()
                    .UseSnakeCaseNamingConvention();
            });
            
            services.AddScoped<IDatabaseContext>(provider => provider.GetService<DatabaseContext>()!);

            return services;
        }
    }
}