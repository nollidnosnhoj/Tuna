using Amazon.S3;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Identity;
using Audiochan.Infrastructure.Persistence;
using Audiochan.Infrastructure.Persistence.Repositories;
using Audiochan.Infrastructure.Shared;
using Audiochan.Infrastructure.Storage;
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
            ConfigureDatabase(services, configuration, isDevelopment);
            ConfigureRepositories(services);
            services.AddAWSService<IAmazonS3>();
            services.AddTransient<IStorageService, AmazonS3Service>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IImageProcessingService, ImageProcessingService>();
            services.AddTransient<ITokenProvider, TokenProvider>();
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            return services;
        }

        private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration,
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
            services.AddScoped<IUnitOfWork>(provider => provider.GetService<ApplicationDbContext>()!);
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddScoped<ITagRepository, TagRepository>();
        }
    }
}