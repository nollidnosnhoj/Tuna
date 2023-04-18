using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Tuna.GraphQl.Configurations
{
    public static class MvcConfiguration
    {
        public static IServiceCollection ConfigureControllers(this IServiceCollection services)
        {
            services
                .AddControllers(configuration =>
                {
                    configuration.Filters.Add(new ProducesAttribute("application/json"));
                });

            return services;
        }

        public static IServiceCollection ConfigureRouting(this IServiceCollection services)
        {
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            return services;
        }
    }
}