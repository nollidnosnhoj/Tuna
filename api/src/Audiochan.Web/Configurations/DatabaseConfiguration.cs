using Audiochan.Core.Interfaces;
using Audiochan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.Web.Configurations
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddDbContext<AudiochanContext>(options =>
            {
                options
                    .UseNpgsql(configuration.GetConnectionString("PostgreSQL"))
                    .UseSnakeCaseNamingConvention();
            });
            
            services.AddScoped<IAudiochanContext>(provider => provider.GetService<AudiochanContext>()!);

            return services;
        }
    }
}