using System;
using Audiochan.API.Extensions.ConfigurationExtensions;
using Audiochan.Core;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using Audiochan.Infrastructure;
using Audiochan.Infrastructure.Storage.AmazonS3;
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
    
    builder.Services.AddApplication(builder.Configuration, builder.Environment);
    builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
    builder.Services.ConfigureAuthentication(builder.Configuration, builder.Environment);
    builder.Services.ConfigureAuthorization();
    builder.Services.AddHttpContextAccessor();
    builder.Services.ConfigureControllers();
    builder.Services.ConfigureGraphQL();
    builder.Services.ConfigureRouting();
    builder.Services.ConfigureRateLimiting();
    builder.Services.ConfigureCors();
    builder.Services.ConfigureSwagger();
    
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(Log.Logger);

    var app = builder.Build();

    app.UseCorsConfig();
    app.UseRateLimiter();
    app.UseSerilogRequestLogging();
    app.UseRouting();
    app.UseAuthentication();
    app.UseRouting();
    app.UseAuthorization();
    app.MapControllers();
    app.MapGraphQL();
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