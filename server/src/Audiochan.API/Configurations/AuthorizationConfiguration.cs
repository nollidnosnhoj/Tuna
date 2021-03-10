using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.API.Configurations
{
    public static class AuthorizationConfiguration
    {
        public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization();

            return services;
        }
    }
}