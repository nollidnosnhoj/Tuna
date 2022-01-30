using AspNetCoreRateLimit;
using Audiochan.API.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.API.Extensions.ConfigurationExtensions
{
    public static class RateLimitConfigExtensions
    {
        public static IServiceCollection ConfigureRateLimiting(this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddInMemoryRateLimiting()
                .Configure<IpRateLimitOptions>(configuration.GetSection("RateLimitingOptions"))
                .Configure<IpRateLimitPolicies>(configuration.GetSection("RateLimitPolicies"))
                .AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>()
                .AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>()
                .AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            return services;
        }

        public static void UseRateLimiting(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomRateLimitingMiddleware>();
        }
    }
}