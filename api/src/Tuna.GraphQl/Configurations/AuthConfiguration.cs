using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tuna.Application.Features.Auth;
using Tuna.GraphQl.Options;
using Tuna.Infrastructure.Identity;

namespace Tuna.GraphQl.Configurations;

public static class AuthConfiguration
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services,
        IConfiguration configuration,
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