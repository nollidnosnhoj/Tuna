using System;
using System.IO;
using Amazon.S3;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Interfaces;
using Audiochan.Infrastructure.Storage;
using Audiochan.Infrastructure.Storage.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Audiochan.Web.Configurations
{
    public static class StorageConfiguration
    {
        public static IServiceCollection ConfigureStorage(this IServiceCollection services, IConfiguration configuration,
            IWebHostEnvironment env)
        {
            var sp = services.BuildServiceProvider();
            if (env.IsDevelopment())
            {
                if (string.IsNullOrWhiteSpace(env.WebRootPath))
                    throw new ArgumentNullException(nameof(env.WebRootPath), 
                        "When is development, parameter must be valid.");
                var basePath = Path.Combine(env.WebRootPath, "uploads");
                var audiochanOptions = configuration.GetSection(nameof(AudiochanOptions)).Get<AudiochanOptions>();
                services.AddSingleton<IStorageService>(_ => new FileStorageService(basePath, audiochanOptions));
            }
            else
            {
                services.Configure<AmazonS3Options>(configuration.GetSection(nameof(AmazonS3Options)));
                services.AddAWSService<IAmazonS3>();
                services.AddTransient<IStorageService, AmazonS3Service>();
            }

            return services;
        }
    }
}