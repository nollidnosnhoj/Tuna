using System.Reflection;
using Audiochan.Core.Common.Pipelines;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.Core
{
    public static class RegisterServices
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DbContextTransactionPipelineBehavior<,>));
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}