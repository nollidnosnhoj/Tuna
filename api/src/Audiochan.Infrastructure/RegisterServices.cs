using Amazon.S3;
using Audiochan.Core.Features.Auth;
using Audiochan.Core.Persistence;
using Audiochan.Core.Security;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Persistence;
using Audiochan.Infrastructure.Identity;
using Audiochan.Infrastructure.Identity.Models;
using Audiochan.Infrastructure.Images.CloudflareImages;
using Audiochan.Infrastructure.Security;
using Audiochan.Infrastructure.Shared;
using Audiochan.Infrastructure.Storage.AmazonS3;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Audiochan.Infrastructure;

public static class RegisterServices
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddCaching(configuration, environment);
        services.AddStorage();
        services.AddImageStorage(configuration);
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddIdentity(configuration);
        services.AddScoped<ITokenService, JsonWebTokenService>();
        return services;
    }

    private static IServiceCollection AddImageStorage(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        return serviceCollection.AddCloudflareImages(configuration);
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

    private static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("IdentityDb"));
        });
        services.AddIdentity<IdUser, IdentityRole>(options =>
            {
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_";
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<IdentityDbContext>();
        services.AddScoped<IAuthService, IdentityAuthService>();
        services.AddScoped<IIdentityService, IdentityUserService>();
        return services;
    }
}