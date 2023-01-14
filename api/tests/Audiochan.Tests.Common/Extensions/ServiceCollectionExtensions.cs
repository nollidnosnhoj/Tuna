using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Audiochan.Tests.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ReplaceService<TService>(this IServiceCollection services, 
        Type serviceType, 
        Func<IServiceProvider, TService> implementation,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient) where TService : class
    {
        services.RemoveAll(serviceType);

        switch (serviceLifetime)
        {
            case ServiceLifetime.Scoped:
                services.AddScoped(implementation);
                break;
            case ServiceLifetime.Singleton:
                services.AddSingleton(implementation);
                break;
            case ServiceLifetime.Transient:
            default:
                services.AddTransient(implementation);
                break;
        }

        return services;
    }
}