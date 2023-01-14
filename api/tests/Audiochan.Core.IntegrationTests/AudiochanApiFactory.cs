using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.API;
using Audiochan.API.Extensions.ConfigurationExtensions;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using Audiochan.Tests.Common.Extensions;
using Audiochan.Tests.Common.Mocks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Xunit;

namespace Audiochan.Core.IntegrationTests;

public class AudiochanApiFactory : WebApplicationFactory<IApiMaker>, IAsyncLifetime
{
    private readonly PostgreSqlTestcontainer _dbContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
            Database = "Audiochan_Test_DB",
            Password = "justsomepassword!%1999",
            Username = "postgres",
            Port = 5432,
        })
        .Build();

    private IConfiguration _configuration = default!;
    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddInMemoryCollection(new[]
                { new KeyValuePair<string, string?>("ConnectionStrings:Database", _dbContainer.ConnectionString) })
            .AddEnvironmentVariables()
            .Build();
        builder.UseConfiguration(_configuration);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            services.ConfigureDatabase(_configuration, true);

            services.ReplaceService(typeof(IDateTimeProvider), 
                _ => new MockDateTimeProvider(DateTime.UtcNow));

            services.ReplaceService(typeof(IStorageService), 
                _ => new MockStorageService(false));
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        _dbConnection = new NpgsqlConnection(_dbContainer.ConnectionString);
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            TablesToIgnore = new Table[] { new("__EFMigrationsHistory") },
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" }
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public async Task DisposeAsync()
    {
        await _dbConnection.CloseAsync();
        await _dbContainer.StopAsync();
    }
}