using Audiochan.Core.Entities.Abstractions;
using Audiochan.Core.Persistence;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Respawn;

namespace Audiochan.IntegrationTests;

[CollectionDefinition(nameof(TestFixture))]
public class TextFixtureCollection : ICollectionFixture<TestFixture> { }

public class TestFixture : IAsyncLifetime
{
    private readonly TestcontainerDatabase _dbContainer = new ContainerBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
            Database = "audiochan_integration_test",
            Username = "test",
            Password = "testpassword"
        }).Build();
    private Respawner _respawner = default!;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly WebApplicationFactory<Program> _factory;

    public TestFixture()
    {
        _factory = new AudiochanTestApplicationFactory(_dbContainer.ConnectionString);
        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    class AudiochanTestApplicationFactory  : WebApplicationFactory<Program>
    {
        private readonly string _connectionString;

        public AudiochanTestApplicationFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new []
                {
                    new KeyValuePair<string, string?>("ConnectionStrings:Database", _connectionString)
                });
            });
        }
    }

    public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        using var scope = _scopeFactory.CreateScope();
        await action(scope.ServiceProvider);
    }

    public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        using var scope = _scopeFactory.CreateScope();
        return await action(scope.ServiceProvider);
    }

    public Task ExecuteDbContextAsync(Func<ApplicationDbContext, Task> action) 
        => ExecuteScopeAsync(sp => action(sp.GetRequiredService<ApplicationDbContext>()));

    public Task ExecuteDbContextAsync(Func<ApplicationDbContext, ValueTask> action) 
        => ExecuteScopeAsync(sp => action(sp.GetRequiredService<ApplicationDbContext>()).AsTask());

    public Task ExecuteDbContextAsync(Func<ApplicationDbContext, IMediator, Task> action) 
        => ExecuteScopeAsync(sp => action(sp.GetRequiredService<ApplicationDbContext>(), sp.GetRequiredService<IMediator>()));

    public Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, Task<T>> action) 
        => ExecuteScopeAsync(sp => action(sp.GetRequiredService<ApplicationDbContext>()));

    public Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, ValueTask<T>> action) 
        => ExecuteScopeAsync(sp => action(sp.GetRequiredService<ApplicationDbContext>()).AsTask());

    public Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, IMediator, Task<T>> action) 
        => ExecuteScopeAsync(sp => action(sp.GetRequiredService<ApplicationDbContext>(), sp.GetRequiredService<IMediator>()));

    public Task InsertAsync<T>(params T[] entities) where T : class
    {
        return ExecuteDbContextAsync(db =>
        {
            foreach (var entity in entities)
            {
                db.Set<T>().Add(entity);
            }
            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity>(TEntity entity) where TEntity : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity, TEntity2>(TEntity entity, TEntity2 entity2) 
        where TEntity : class
        where TEntity2 : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity, TEntity2, TEntity3>(TEntity entity, TEntity2 entity2, TEntity3 entity3) 
        where TEntity : class
        where TEntity2 : class
        where TEntity3 : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);
            db.Set<TEntity3>().Add(entity3);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4>(TEntity entity, TEntity2 entity2, TEntity3 entity3, TEntity4 entity4) 
        where TEntity : class
        where TEntity2 : class
        where TEntity3 : class
        where TEntity4 : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);
            db.Set<TEntity3>().Add(entity3);
            db.Set<TEntity4>().Add(entity4);

            return db.SaveChangesAsync();
        });
    }

    public Task<T?> FindAsync<T, TKey>(TKey id) 
        where T : Entity<TKey>
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        return ExecuteDbContextAsync(db => db.Set<T>().FindAsync(id).AsTask());
    }

    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        return ExecuteScopeAsync(sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();

            return mediator.Send(request);
        });
    }

    public Task SendAsync(IRequest request)
    {
        return ExecuteScopeAsync(sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();

            return mediator.Send(request);
        });
    }

    public async Task InitializeAsync()
    {
        var connectionString = _configuration.GetConnectionString("Database");
        _respawner = await Respawner.CreateAsync(connectionString!, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public", "identity" }
        });
        await _respawner.ResetAsync(connectionString!);
    }

    public Task DisposeAsync()
    {
        _factory.Dispose();
        return Task.CompletedTask;
    }
}