using Amazon.S3;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Caching;
using Audiochan.Infrastructure.Identity;
using Audiochan.Infrastructure.Persistence;
using Audiochan.Infrastructure.Persistence.Repositories;
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
                .ConfigurePersistence(configuration, environment)
                .ConfigureStorageService()
                .AddTransient<IIdentityService, IdentityService>()
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
        
        private static IServiceCollection ConfigurePersistence(this IServiceCollection services, 
            IConfiguration configuration, IHostEnvironment env)
        {
            services.AddDbContext<ApplicationDbContext>(o =>
            {
                o.UseNpgsql(configuration.GetConnectionString("Database"));
                o.UseSnakeCaseNamingConvention();
                if (env.IsDevelopment())
                {
                    o.EnableSensitiveDataLogging();
                }
            });
            services.AddScoped<IAudioRepository, AudioRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}