using System.Text.Json;
using Audiochan.API.Extensions.ConfigurationExtensions;
using Audiochan.API.Middlewares;
using Audiochan.API.Services;
using Audiochan.Core;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Infrastructure;
using Audiochan.Infrastructure.Storage.AmazonS3;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Audiochan.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AmazonS3Settings>(Configuration.GetSection(nameof(AmazonS3Settings)));
            services.Configure<MediaStorageSettings>(Configuration.GetSection(nameof(MediaStorageSettings)));
            services.Configure<IdentitySettings>(Configuration.GetSection(nameof(IdentitySettings)));

            var jsonSerializerOptions = new JsonSerializerOptions()
            {
                IgnoreNullValues = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = false
            };

            services.AddMemoryCache();
            services.AddInfrastructure(Configuration, Environment);
            services.AddCore();
            services.Configure<JsonSerializerOptions>(options =>
            {
                options.IgnoreNullValues = jsonSerializerOptions.IgnoreNullValues;
                options.PropertyNamingPolicy = jsonSerializerOptions.PropertyNamingPolicy;
                options.PropertyNameCaseInsensitive = jsonSerializerOptions.PropertyNameCaseInsensitive;
            });
            services.ConfigureAuthentication(Environment);
            services.ConfigureAuthorization();
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.ConfigureControllers(jsonSerializerOptions);
            services.ConfigureRouting();
            services.ConfigureRateLimiting(Configuration);
            services.ConfigureCors();
            services.ConfigureSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCorsConfig();
            app.UseRateLimiting();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSwaggerConfig();
        }
    }
}