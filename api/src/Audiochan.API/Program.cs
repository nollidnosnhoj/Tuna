using System;
using System.Reflection;
using Audiochan.API.Configurations;
using Audiochan.Common.Mediatr.Pipelines;
using Audiochan.Core;
using Audiochan.Core.Persistence;
using Audiochan.Core.Persistence.Pipelines;
using Audiochan.Infrastructure;
using Audiochan.Infrastructure.Storage.AmazonS3;
using FluentValidation;
using HashidsNet;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
    builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection(nameof(ApplicationSettings)));
    
    builder.Services.AddMediatR(config => 
        config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DbContextTransactionPipelineBehavior<,>));
    builder.Services.AddSingleton<IHashids>(_ => new Hashids(salt: "audiochan", minHashLength: 7));
    
    builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
    
    builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(o =>
    {
        if (builder.Environment.IsDevelopment())
        {
            o.UseInMemoryDatabase("devdb");
        }
        else
        {
            o.UseNpgsql(builder.Configuration.GetConnectionString("Database"));
        }
        o.UseSnakeCaseNamingConvention();
        if (builder.Environment.IsDevelopment())
        {
            o.EnableSensitiveDataLogging();
        }
    });
    
    builder.Services.ConfigureAuthentication(builder.Configuration, builder.Environment);
    builder.Services.ConfigureAuthorization();
    builder.Services.AddHttpContextAccessor();
    builder.Services.ConfigureControllers();
    builder.Services.AddAudiochanGraphQl();
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