using System;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.API.Extensions.ConfigurationExtensions
{
    public static class RateLimitConfigExtensions
    {
        public static IServiceCollection ConfigureRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                        context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
                        _ => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 50,
                            QueueLimit = 0,
                            Window = TimeSpan.FromMinutes(1)
                        });
                });
            });

            return services;
        }
    }
}