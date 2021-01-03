using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Audiochan.Core;
using Audiochan.Core.Interfaces;
using Audiochan.Web.Extensions;
using Audiochan.Infrastructure;
using Audiochan.Infrastructure.Storage;
using Audiochan.Infrastructure.Storage.Options;
using Audiochan.Web.Configurations;
using Audiochan.Web.Filters;
using Audiochan.Web.Middlewares;
using Audiochan.Web.Services;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Audiochan.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        private const string CorsPolicyName = "CORS_POLICY";

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCoreServices();
            services.AddInfraServices(Configuration, Environment.IsProduction(), Environment.WebRootPath);
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddIdentityAndAuth(Configuration);
            services.AddHttpContextAccessor();
            
            services
                .AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                    options.Filters.Add<ValidationFilter>();
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .AddFluentValidation(c =>
                    c.ValidatorFactoryType = typeof(HttpContextServiceProviderValidatorFactory));

            services.AddRouting(options => options.LowercaseUrls = true);
            services.ConfigureCors(CorsPolicyName, Configuration);
            services.ConfigureSwagger(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                }
            });
            
            app.UseRouting();
            
            app.UseCors(CorsPolicyName);
            
            app.UseAuthentication();
            
            app.UseAuthorization();

            app.UseMvc();
            
            app.UseSwagger();
            
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Audiochan API Version 1");
            });
        }
    }
}
