using Amazon.S3;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Identity;
using Audiochan.Infrastructure.Persistence;
using Audiochan.Infrastructure.Shared;
using Audiochan.Infrastructure.Storage.AmazonS3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.Infrastructure
{
    public static class RegisterServices
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration,
            bool isDevelopment)
        {
            services.ConfigurePersistence(configuration, isDevelopment);
            services.ConfigureStorageService();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IImageProcessingService, ImageProcessingService>();
            services.AddTransient<ITokenProvider, TokenProvider>();
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            return services;
        }

        private static IServiceCollection ConfigureStorageService(this IServiceCollection services)
        {
            services.AddAWSService<IAmazonS3>();
            services.AddTransient<IStorageService, AmazonS3Service>();
            return services;
        }
        
        private static IServiceCollection ConfigurePersistence(this IServiceCollection services, IConfiguration configuration,
            bool isDevelopment)
        {
            services.AddDbContext<ApplicationDbContext>(o =>
            {
                o.UseNpgsql(configuration.GetConnectionString("Database"));
                o.UseSnakeCaseNamingConvention();
                if (isDevelopment)
                {
                    o.EnableSensitiveDataLogging();
                }
            });
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}