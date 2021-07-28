using Amazon.S3;
using Audiochan.Core;
using Audiochan.Core.Interfaces;
using Audiochan.Infrastructure.Caching;
using Audiochan.Infrastructure.Shared;
using Audiochan.Infrastructure.Storage.AmazonS3;
using Microsoft.EntityFrameworkCore;
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
            return services
                .ConfigureCaching(configuration, environment)
                .ConfigureStorageService()
                .AddTransient<IImageProcessingService, ImageProcessingService>()
                .AddTransient<ITokenProvider, TokenProvider>()
                .AddTransient<IDateTimeProvider, DateTimeProvider>()
                .AddTransient<INanoidGenerator, NanoidGenerator>();
        }

        private static IServiceCollection ConfigureCaching(this IServiceCollection services, IConfiguration config, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                services.AddSingleton<ICacheService, MemoryCacheService>();
            }
            else
            {
                services.AddSingleton<IConnectionMultiplexer>(_ =>
                    ConnectionMultiplexer.Connect("localhost"));
                services.AddSingleton<ICacheService, RedisCacheService>();
            }
            return services;
        }

        private static IServiceCollection ConfigureStorageService(this IServiceCollection services)
        {
            services.AddAWSService<IAmazonS3>();
            services.AddTransient<IStorageService, AmazonS3Service>();
            return services;
        }
    }
}