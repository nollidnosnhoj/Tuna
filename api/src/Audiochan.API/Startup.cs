using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Audiochan.API.Extensions.ConfigurationExtensions;
using Audiochan.API.Middlewares;
using Audiochan.API.Middlewares.Pipelines;
using Audiochan.API.Services;
using Audiochan.Core;
using Audiochan.Core.Services;
using Audiochan.Infrastructure;
using Audiochan.Infrastructure.Storage.AmazonS3;
using FluentValidation;
using MediatR;
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
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = false
            };

            services.AddMemoryCache();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DbContextTransactionPipelineBehavior<,>));
            services.AddDatabase(Configuration, Environment);
            services.AddInfrastructure(Configuration, Environment);
            services.Configure<JsonSerializerOptions>(options =>
            {
                options.DefaultIgnoreCondition = jsonSerializerOptions.DefaultIgnoreCondition;
                options.PropertyNamingPolicy = jsonSerializerOptions.PropertyNamingPolicy;
                options.PropertyNameCaseInsensitive = jsonSerializerOptions.PropertyNameCaseInsensitive;
            });
            services.ConfigureAuthentication(Configuration, Environment);
            services.ConfigureAuthorization();
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.ConfigureControllers(jsonSerializerOptions);
            services.ConfigureRouting();
            services.ConfigureRateLimiting();
            services.ConfigureCors();
            services.ConfigureSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCorsConfig();
            app.UseRateLimiter();
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