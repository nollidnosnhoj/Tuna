using System;
using System.Reflection;
using FluentValidation;
using HashidsNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Tuna.Application;
using Tuna.GraphQl.Configurations;
using Tuna.Infrastructure;
using Tuna.Infrastructure.Storage.AmazonS3;

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
    builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    builder.Services.AddMediatrPipelines();
    builder.Services.AddSingleton<IHashids>(_ => new Hashids("saltytuna", 7));
    builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
    builder.Services.AddPersistence(builder.Configuration, builder.Environment);
    builder.Services.ConfigureAuthentication(builder.Configuration, builder.Environment);
    builder.Services.ConfigureAuthorization();
    builder.Services.AddHttpContextAccessor();
    builder.Services.ConfigureControllers();
    builder.Services.AddTunaGraphQl();
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