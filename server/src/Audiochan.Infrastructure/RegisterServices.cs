using Amazon.S3;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Infrastructure.Image;
using Audiochan.Infrastructure.Persistence;
using Audiochan.Infrastructure.Persistence.Repositories;
using Audiochan.Infrastructure.Search;
using Audiochan.Infrastructure.Security;
using Audiochan.Infrastructure.Shared;
using Audiochan.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.Infrastructure
{
    public static class RegisterServices
    {
        public static IServiceCollection AddServicesFromInfrastructure(this IServiceCollection services,
            IConfiguration configuration,
            bool isDevelopment)
        {
            ConfigureDatabase(services, configuration, isDevelopment);
            ConfigureRepositories(services);
            services.AddAWSService<IAmazonS3>();
            services.AddTransient<IStorageService, AmazonS3Service>();
            services.AddTransient<ISearchService, DatabaseSearchService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<ITokenProvider, TokenProvider>();
            services.AddTransient<IDateTimeProvider, NodaTimeProvider>();
            services.AddTransient<CleanupService>();
            return services;
        }

        private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration,
            bool isDevelopment)
        {
            services.AddDbContext<ApplicationDbContext>(o =>
            {
                o.UseNpgsql(configuration.GetConnectionString("Database"), npgo => 
                {
                    npgo.UseNodaTime();
                });
                o.UseSnakeCaseNamingConvention();
                if (isDevelopment)
                {
                    o.EnableSensitiveDataLogging();
                }
            });

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddScoped<ITagRepository, TagRepository>();
        }
    }
}