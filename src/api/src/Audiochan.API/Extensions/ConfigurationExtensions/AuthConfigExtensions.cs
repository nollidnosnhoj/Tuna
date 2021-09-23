using Audiochan.API.Models;
using Audiochan.API.Services;
using Audiochan.Core.Common.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Audiochan.API.Extensions.ConfigurationExtensions
{
    public static class AuthConfigExtensions
    {
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration, 
            IWebHostEnvironment environment)
        {
            services.AddSingleton<ITicketStore, DistributedCacheTicketStore>();

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

                    options.Events.OnRedirectToLogin = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        var response = ErrorApiResponse.Unauthorized();
                        await context.Response.WriteAsJsonAsync(response);
                        await context.Response.Body.FlushAsync();
                    };

                    options.Events.OnRedirectToAccessDenied = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        var response = ErrorApiResponse.Forbidden();
                        await context.Response.WriteAsJsonAsync(response);
                        await context.Response.Body.FlushAsync();
                    };
                });
            
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

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