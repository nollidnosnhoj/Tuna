using System;
using Audiochan.API.Configurations;
using Audiochan.Core;
using Audiochan.Infrastructure;
using Audiochan.Infrastructure.Storage.AmazonS3;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
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
    builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection(nameof(ApplicationSettings)));
    
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