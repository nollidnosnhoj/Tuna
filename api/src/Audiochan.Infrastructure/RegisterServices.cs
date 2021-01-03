using System;
using System.IO;
using Amazon.S3;
using Audiochan.Core.Interfaces;
using Audiochan.Infrastructure.Data;
using Audiochan.Infrastructure.Security;
using Audiochan.Infrastructure.Shared;
using Audiochan.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.Infrastructure
{
    public static class RegisterServices
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services, 
            IConfiguration configuration, bool isProduction = false, string wwwRoot = "")
        {
            services.AddDbContext<AudiochanContext>(options =>
            {
                options
                    .UseNpgsql(configuration.GetConnectionString("PostgreSQL"))
                    .UseSnakeCaseNamingConvention();
            });

            if (isProduction)
            {
                services.AddAWSService<IAmazonS3>();
                services.AddTransient<IStorageService, AmazonS3Service>();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(wwwRoot))
                    throw new ArgumentNullException(nameof(wwwRoot), 
                        "When is development, parameter must be valid.");
                    
                var wwwroot = wwwRoot;
                var basePath = Path.Combine(wwwroot, "uploads");
                Directory.CreateDirectory(basePath);
                services.AddSingleton<IStorageService>(_ => new FileStorageService(basePath));
            }
            
            services.AddScoped<IAudiochanContext>(provider => provider.GetService<AudiochanContext>()!);
            services.AddTransient<IAudioMetadataService, AudioMetadataService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IDateTimeService, DateTimeService>();
                
            return services;
        }
    }
}