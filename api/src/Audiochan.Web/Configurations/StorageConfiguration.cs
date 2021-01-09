using System;
using System.IO;
using Amazon.S3;
using Audiochan.Core.Interfaces;
using Audiochan.Infrastructure.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Audiochan.Web.Configurations
{
    public static class StorageConfiguration
    {
        public static IServiceCollection ConfigureStorage(this IServiceCollection services, IWebHostEnvironment env)
        {
            if (env.IsProduction())
            {
                services.AddAWSService<IAmazonS3>();
                services.AddTransient<IStorageService, AmazonS3Service>();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(env.WebRootPath))
                    throw new ArgumentNullException(nameof(env.WebRootPath), 
                        "When is development, parameter must be valid.");
                
                var basePath = Path.Combine(env.WebRootPath, "uploads");
                Directory.CreateDirectory(basePath);
                services.AddSingleton<IStorageService>(_ => new FileStorageService(basePath));
            }

            return services;
        }
    }
}