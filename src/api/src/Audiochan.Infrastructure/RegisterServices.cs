using Amazon.S3;
using Audiochan.Core.Interfaces;
using Audiochan.Infrastructure.Caching;
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
            if (environment.IsDevelopment())
            {
                services.AddSingleton<ICacheService, MemoryCacheService>();
            }
            else
            {
                services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect("localhost"));
                services.AddSingleton<ICacheService, RedisCacheService>();
            }
            
            services.AddAWSService<IAmazonS3>();
            services.AddTransient<IStorageService, AmazonS3Service>();
            services.AddTransient<ISlugGenerator, SlugGenerator>();
            services.AddTransient<IImageUploadService, ImageUploadService>();
            services.AddTransient<ITokenProvider, TokenProvider>();
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddTransient<INanoidGenerator, NanoidGenerator>();
            services.AddTransient<IPasswordHasher, BCryptHasher>();
            return services;
        }
    }
}