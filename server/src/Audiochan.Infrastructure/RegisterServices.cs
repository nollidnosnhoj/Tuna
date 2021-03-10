using Amazon.S3;
using Audiochan.Core.Interfaces;
using Audiochan.Infrastructure.Image;
using Audiochan.Infrastructure.Persistence;
using Audiochan.Infrastructure.Persistence.Repositories;
using Audiochan.Infrastructure.Search;
using Audiochan.Infrastructure.Security;
using Audiochan.Infrastructure.Shared;
using Audiochan.Infrastructure.Storage;
using Audiochan.Infrastructure.Upload;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.Infrastructure
{
    public static class RegisterServices
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services,
            IConfiguration configuration,
            bool isDevelopment)
        {
            ConfigureDatabase(services, configuration, isDevelopment);
            ConfigureRepositories(services);
            services.AddAWSService<IAmazonS3>();
            services.AddTransient<IStorageService, AmazonS3Service>();
            services.AddTransient<ISearchService, DatabaseSearchService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<IUploadService, UploadService>();
            return services;
        }

        private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration,
            bool isDevelopment)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("Database"));
                options.UseSnakeCaseNamingConvention();
                if (isDevelopment)
                {
                    options.EnableSensitiveDataLogging();
                }
            });

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddScoped<IGenreRepository, GenreRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
        }
    }
}