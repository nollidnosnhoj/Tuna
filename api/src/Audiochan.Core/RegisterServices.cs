using System.Reflection;
using Audiochan.Core.Common.Pipelines;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.Core
{
    public static class RegisterServices
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            return services
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                .AddMediatR(Assembly.GetExecutingAssembly())
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(DbTransactionPipelineBehavior<,>));
        }
    }
}