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
        public static IServiceCollection AddInfraServices(this IServiceCollection services)
        {
            services.AddTransient<IAudioMetadataService, AudioMetadataService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IDateTimeService, DateTimeService>();
                
            return services;
        }
    }
}