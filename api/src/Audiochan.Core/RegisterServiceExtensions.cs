using Audiochan.Core.Persistence;
using Audiochan.Core.Persistence.Pipelines;
using Audiochan.Shared.Mediatr.Pipelines;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Audiochan.Core;

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
            if (environment.IsDevelopment())
            {
                o.EnableSensitiveDataLogging();
            }
        });
    }
}