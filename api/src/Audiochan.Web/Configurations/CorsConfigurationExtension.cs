using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.Web.Configurations
{
    public static class CorsConfigurationExtension
    {
        public static void ConfigureCors(this IServiceCollection services, string corsPolicyName, 
            IConfiguration configuration)
        {
            var frontendUrl = configuration.GetValue<string>("FrontendUrl");

            services.AddCors(options =>
            {
                options.AddPolicy(corsPolicyName, builder =>
                {
                    if (string.IsNullOrWhiteSpace(frontendUrl))
                        builder.SetIsOriginAllowed(origin => true);
                    else
                        builder.WithOrigins(frontendUrl);
                    
                    builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
        }
    }
}