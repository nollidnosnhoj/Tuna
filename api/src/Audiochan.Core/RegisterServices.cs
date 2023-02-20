using System.Reflection;
using Audiochan.Common.Mediatr.Pipelines;
using Audiochan.Common.Services;
using Audiochan.Core.Persistence;
using Audiochan.Core.Persistence.Pipelines;
using Audiochan.Core.Services;
using FluentValidation;
using HashidsNet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Audiochan.Core;

public static class RegisterServices
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DbContextTransactionPipelineBehavior<,>));
        services.AddTransient<IImageService, ImageService>();
        services.AddSingleton<IHashids>(_ => new Hashids(salt: "audiochan", minHashLength: 7));
        services.AddPersistence(configuration, environment);
        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddPooledDbContextFactory<ApplicationDbContext>(o =>
        {
            o.UseNpgsql(configuration.GetConnectionString("Database"));
            o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            o.UseSnakeCaseNamingConvention();
            if (environment.IsDevelopment())
            {
                o.EnableSensitiveDataLogging();
            }
        });
        return services;
    }
}