using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Audiochan.API.Extensions.ConfigurationExtensions;
using Audiochan.API.Middlewares;
using Audiochan.API.Services;
using Audiochan.Common.Mediatr.Pipelines;
using Audiochan.Core;
using Audiochan.Core.Persistence;
using Audiochan.Core.Persistence.Pipelines;
using Audiochan.Core.Services;
using Audiochan.Infrastructure;
using Audiochan.Infrastructure.Storage.AmazonS3;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder();

    builder.Services.Configure<AmazonS3Settings>(builder.Configuration.GetSection(nameof(AmazonS3Settings)));
    builder.Services.Configure<MediaStorageSettings>(builder.Configuration.GetSection(nameof(MediaStorageSettings)));
    builder.Services.Configure<IdentitySettings>(builder.Configuration.GetSection(nameof(IdentitySettings)));
    
    builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DbContextTransactionPipelineBehavior<,>));
    builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
    builder.Services.ConfigureAuthentication(builder.Configuration, builder.Environment);
    builder.Services.ConfigureAuthorization();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.ConfigureControllers();
    builder.Services.ConfigureDatabase(builder.Configuration, builder.Environment.IsDevelopment());
    builder.Services.ConfigureRouting();
    builder.Services.ConfigureRateLimiting();
    builder.Services.ConfigureCors();
    builder.Services.ConfigureSwagger();
    
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(Log.Logger);

    var app = builder.Build();

    app.UseCorsConfig();
    app.UseRateLimiter();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseSerilogRequestLogging();
    app.UseRouting();
    app.UseAuthentication();
    app.UseRouting();
    app.UseAuthorization();
    app.MapControllers();
    app.UseSwaggerConfig();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var env = services.GetRequiredService<IWebHostEnvironment>();
        var context = services.GetRequiredService<ApplicationDbContext>();

        Log.Information("Migrating database...");
        await context.Database.MigrateAsync();

        if (env.IsDevelopment())
        {
            Log.Information("Seeding users and default audios.");
            var pwHasher = services.GetRequiredService<IPasswordHasher>();
            var userId = await ApplicationDbSeeder.UserSeedAsync(context, pwHasher);
            if (userId > 0)
            {
                await ApplicationDbSeeder.AudioSeedAsync(context, userId);
            }
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}