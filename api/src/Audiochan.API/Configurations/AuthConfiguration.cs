using System.Threading.Tasks;
using Audiochan.API.Services;
using Audiochan.Core.Features.Auth;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Audiochan.API.Configurations
{
    public static class AuthConfiguration
    {
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration, 
            IWebHostEnvironment environment)
        {
            services.AddSingleton<ITicketStore, DistributedCacheTicketStore>();
            services.AddTransient<IAuthService, AuthService>();

            services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
                .Configure<ITicketStore>((options, ticketStore) =>
                {
                    options.Cookie.Name = "auth.cookie";
                    options.SessionStore = ticketStore;
                    options.Cookie.SameSite = environment.IsProduction()
                        ? SameSiteMode.Lax
                        : SameSiteMode.Unspecified;

                    options.Cookie.SecurePolicy = environment.IsProduction()
                        ? CookieSecurePolicy.Always
                        : CookieSecurePolicy.None;

                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };

                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    };
                });
            
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            return services;
        }

        public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization();

            return services;
        }
    }
}