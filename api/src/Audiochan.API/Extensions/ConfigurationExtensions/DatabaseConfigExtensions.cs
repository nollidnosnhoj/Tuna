using Audiochan.Core.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Audiochan.API.Extensions.ConfigurationExtensions;

public static class DatabaseConfigExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration,
        bool isDevelopment)
    {
        services.AddDbContext<ApplicationDbContext>(o =>
        {
            o.UseNpgsql(configuration.GetConnectionString("Database"));
            o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            o.UseSnakeCaseNamingConvention();
            if (isDevelopment)
            {
                o.EnableSensitiveDataLogging();
            }
        });
        return services;
    }
}