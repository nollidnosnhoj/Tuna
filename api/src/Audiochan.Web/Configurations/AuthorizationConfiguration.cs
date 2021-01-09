using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.Web.Configurations
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