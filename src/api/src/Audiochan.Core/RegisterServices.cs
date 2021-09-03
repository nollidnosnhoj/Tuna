using System.Reflection;
using Audiochan.Core.Common.Pipelines;
using Audiochan.Core.Services;
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
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddBehaviorPipelines();
            services.AddTransient<IAudioUploadService, AudioUploadService>();
            return services;
        }

        private static IServiceCollection AddBehaviorPipelines(this IServiceCollection services)
        {
            return services
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(DbContextTransactionPipelineBehavior<,>));
        }
        
        
    }
}