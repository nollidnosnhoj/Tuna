using System;
using System.Text;
using Audiochan.API.Services;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Audiochan.API.Extensions.ConfigurationExtensions
{
    public static class AuthConfigExtensions
    {
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services,
            IWebHostEnvironment environment)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "auth.cookie";
                    options.Cookie.SecurePolicy = environment.IsProduction()
                        ? CookieSecurePolicy.Always
                        : CookieSecurePolicy.None;
                });

            services.AddTransient<ICurrentUserService, CurrentUserService>();

            return services;
        }

        public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization();

            return services;
        }
    }
}