using System.Reflection;
using System.Text.Json;
using Audiochan.API.Configurations;
using Audiochan.API.Middlewares;
using Audiochan.API.Services;
using Audiochan.Core;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Pipelines;
using Audiochan.Core.Settings;
using Audiochan.Infrastructure;
using Audiochan.Infrastructure.Storage.Options;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.Configure<JwtSettings>(Configuration.GetSection(nameof(JwtSettings)));
            services.Configure<IdentitySettings>(Configuration.GetSection(nameof(IdentitySettings)));
            
            var jsonSerializerOptions = new JsonSerializerOptions()
            {
                
                IgnoreNullValues = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = false
            };

            services
                .AddMemoryCache()
                .AddMediatR(Assembly.GetExecutingAssembly())
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(DbContextTransactionPipelineBehavior<,>))
                .AddInfraServices(Configuration, Environment.IsDevelopment())
                .Configure<JsonSerializerOptions>(options =>
                {
                    options.IgnoreNullValues = jsonSerializerOptions.IgnoreNullValues;
                    options.PropertyNamingPolicy = jsonSerializerOptions.PropertyNamingPolicy;
                    options.PropertyNameCaseInsensitive = jsonSerializerOptions.PropertyNameCaseInsensitive;
                })
                .ConfigureIdentity(Configuration)
                .ConfigureAuthentication(Configuration)
                .ConfigureAuthorization()
                .AddHttpContextAccessor()
                .AddScoped<ICurrentUserService, CurrentUserService>()
                .ConfigureControllers(jsonSerializerOptions)
                .ConfigureRouting()
                .ConfigureRateLimiting(Configuration)
                .ConfigureCors()
                .ConfigureSwagger(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCorsConfig();
            app.UseRateLimiting();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSwaggerConfig();
        }
    }
}