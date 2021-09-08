using Amazon.S3;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Domain.Entities;
using Audiochan.Infrastructure.Caching;
using Audiochan.Infrastructure.Persistence;
using Audiochan.Infrastructure.Persistence.Repositories;
using Audiochan.Infrastructure.Persistence.Repositories.Abstractions;
using Audiochan.Infrastructure.Search;
using Audiochan.Infrastructure.Security;
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
            services.AddCaching(configuration, environment);
            services.AddStorage();
            services.AddSearch();
            services.AddTransient<ISlugGenerator, SlugGenerator>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<ITokenProvider, TokenProvider>();
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddTransient<IRandomIdGenerator, NanoidGenerator>();
            services.AddTransient<IPasswordHasher, BCryptHasher>();
            services.AddPersistence(configuration, environment);
            return services;
        }

        private static IServiceCollection AddPersistence(this IServiceCollection services, 
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

            services.AddScoped(typeof(IEntityRepository<>), typeof(EfRepository<>));
            services.AddScoped<IAudioRepository, AudioRepository>();
            services.AddScoped<IPlaylistRepository, PlaylistRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            return services;
        }

        private static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            if (!environment.IsProduction())
            {
                services.AddSingleton<ICacheService, InMemoryCacheService>();
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