using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tuna.Application.Persistence;
using Tuna.Application.Persistence.Pipelines;
using Tuna.Shared.Mediatr.Pipelines;

namespace Tuna.Application;

public static class RegisterServiceExtensions
{
    public static IServiceCollection AddMediatrPipelines(this IServiceCollection services)
    {
        return services
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(DbContextTransactionPipelineBehavior<,>));
    }

    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        return services.AddPooledDbContextFactory<ApplicationDbContext>(o =>
        {
            o.UseNpgsql(configuration.GetConnectionString("Database"));
            o.UseSnakeCaseNamingConvention();
            if (environment.IsDevelopment()) o.EnableSensitiveDataLogging();
        });
    }
}