using Audiochan.Core.Interfaces;
using Audiochan.Infrastructure.AudioUpload;
using Audiochan.Infrastructure.ImageSharp;
using Audiochan.Infrastructure.Security;
using Audiochan.Infrastructure.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.Infrastructure
{
    public static class RegisterServices
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services)
        {
            services.AddTransient<IAudioUploadService, AudioUploadService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<IImageService, ImageService>();
            return services;
        }
    }
}