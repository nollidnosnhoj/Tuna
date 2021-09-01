using Amazon.S3;
using Audiochan.Core.Interfaces;
using Audiochan.Infrastructure.Caching;
using Audiochan.Infrastructure.Search;
using Audiochan.Infrastructure.Security;
using Audiochan.Infrastructure.Shared;
using Audiochan.Infrastructure.Storage.AmazonS3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace Audiochan.Infrastructure
{
    public static class RegisterServices
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment)
        {
            services.AddCaching(configuration, environment);
            services.AddStorage();
            services.AddSearch();
            services.AddTransient<ISlugGenerator, SlugGenerator>();
            services.AddTransient<IImageUploadService, ImageUploadService>();
            services.AddTransient<ITokenProvider, TokenProvider>();
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddTransient<IRandomIdGenerator, NanoidGenerator>();
            services.AddTransient<IPasswordHasher, BCryptHasher>();
            return services;
        }

        private static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                services.AddSingleton<ICacheService, MemoryCacheService>();
            }
            else
            {
                services.AddSingleton<IConnectionMultiplexer>(_ => 
                    ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));
                services.AddSingleton<ICacheService, RedisCacheService>();
            }

            return services;
        }

        private static IServiceCollection AddStorage(this IServiceCollection services)
        {
            services.AddAWSService<IAmazonS3>();
            services.AddTransient<IStorageService, AmazonS3Service>();
            return services;
        }

        private static IServiceCollection AddSearch(this IServiceCollection services)
        {
            services.AddTransient<ISearchService, PostgresSearchService>();
            return services;
        }
    }
}