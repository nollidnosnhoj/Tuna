using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.API.Configurations
{
    public static class RoutingConfiguration
    {
        public static IServiceCollection ConfigureRouting(this IServiceCollection services)
        {
            services.AddRouting(options => options.LowercaseUrls = true);

            return services;
        }
    }
}