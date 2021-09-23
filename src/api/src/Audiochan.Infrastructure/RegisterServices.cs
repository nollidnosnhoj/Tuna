using Amazon.S3;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Infrastructure.Persistence;
using Audiochan.Infrastructure.Persistence.Repositories;
using Audiochan.Infrastructure.Persistence.Repositories.Abstractions;
using Audiochan.Infrastructure.Search;
using Audiochan.Infrastructure.Security;
using Audiochan.Infrastructure.Shared;
using Audiochan.Infrastructure.Storage.AmazonS3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddTransient<IRandomIdGenerator, NanoidGenerator>();
            services.AddTransient<IPasswordHasher, BCryptHasher>();
            services.AddPersistence(configuration, environment);
            return services;
        }

        // public static AuthenticationBuilder AddOAuthProviders(this AuthenticationBuilder builder,
        //     AuthenticationSettings configuration)
        // {
        //     builder.AddDiscord(options =>
        //     {
        //         options.ClientId = configuration.DiscordClientId;
        //         options.ClientSecret = configuration.DiscordClientSecret;
        //     });
        //
        //     return builder;
        // }

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
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            return services;
        }

        private static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            if (!environment.IsProduction())
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration.GetConnectionString("Redis");
                    options.InstanceName = "audiochan_redis";
                });
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