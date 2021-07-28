using System.Reflection;
using Audiochan.Core.Common.Pipelines;
using Audiochan.Core.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Audiochan.Core
{
    public static class RegisterServices
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration config, IHostEnvironment env)
        {
            return services
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                .AddMediatR(Assembly.GetExecutingAssembly())
                .AddBehaviorPipelines()
                .ConfigurePersistence(config, env);
        }

        private static IServiceCollection AddBehaviorPipelines(this IServiceCollection services)
        {
            return services
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        }
        
        private static IServiceCollection ConfigurePersistence(this IServiceCollection services, 
            IConfiguration configuration, IHostEnvironment env)
        {
            services.AddDbContext<ApplicationDbContext>(o =>
            {
                o.UseNpgsql(configuration.GetConnectionString("Database"));
                o.UseSnakeCaseNamingConvention();
                if (env.IsDevelopment())
                {
                    o.EnableSensitiveDataLogging();
                }
            });
            return services;
        }
    }
}