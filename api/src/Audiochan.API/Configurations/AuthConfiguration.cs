using Audiochan.API.Options;
using Audiochan.Core.Features.Auth;
using Audiochan.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.API.Configurations
{
    public static class AuthConfiguration
    {
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration, 
            IWebHostEnvironment environment)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
            services.ConfigureOptions<JwtConfigureOptions>();
            services.ConfigureOptions<JwtBearerConfigureOptions>();
            
            services.AddTransient<IAuthService, AuthService>();

            return services;
        }

        public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization();

            return services;
        }
    }
}