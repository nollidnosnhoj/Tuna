using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Commons.Pipelines;
using Audiochan.Application.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Audiochan.Application;

public static class RegisterServices
{
    public static IServiceCollection AddApplication(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DbContextTransactionPipelineBehavior<,>));
        services.AddDatabase(configuration, environment);
        return services;
    }
    
    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddPooledDbContextFactory<ApplicationDbContext>((o) =>
        {
            o.UseNpgsql(configuration.GetConnectionString("Database"));
            o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            o.UseSnakeCaseNamingConvention();
            if (environment.IsDevelopment())
            {
                o.EnableSensitiveDataLogging();
            }
        });
            
        services.AddScoped(sp =>
        {
            var factory = sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            return factory.CreateDbContext();
        });
        return services;
    }
}