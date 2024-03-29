﻿using HotChocolate;
using HotChocolate.Execution;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Respawn;
using Testcontainers.PostgreSql;
using Tuna.Application.Persistence;
using Tuna.Domain.Entities.Abstractions;

namespace Tuna.IntegrationTests;

[CollectionDefinition(nameof(TestFixture))]
public class TextFixtureCollection : ICollectionFixture<TestFixture>
{
}

public class TestFixture : IAsyncLifetime
{
    private readonly IConfiguration _configuration;

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("tuna_integration_test")
        .WithUsername("test")
        .WithPassword("testpassword")
        .Build();

    private readonly WebApplicationFactory<Program> _factory;
    private readonly IServiceScopeFactory _scopeFactory;
    private Respawner _respawner = default!;

    public TestFixture()
    {
        _factory = new TestApplicationFactory(_dbContainer.GetConnectionString());
        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
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

    public async Task<IExecutionResult> ExecuteGraphQlRequestAsync(
        Action<IQueryRequestBuilder> configure,
        CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var executor = scope.ServiceProvider.GetRequiredService<RequestExecutorProxy>();
        var requestBuilder = new QueryRequestBuilder();
        configure(requestBuilder);
        var request = requestBuilder.Create();
        return await executor.ExecuteAsync(request, cancellationToken);
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
    {
        return ExecuteScopeAsync(sp => action(sp.GetRequiredService<ApplicationDbContext>()));
    }

    public Task ExecuteDbContextAsync(Func<ApplicationDbContext, ValueTask> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetRequiredService<ApplicationDbContext>()).AsTask());
    }

    public Task ExecuteDbContextAsync(Func<ApplicationDbContext, IMediator, Task> action)
    {
        return ExecuteScopeAsync(sp =>
            action(sp.GetRequiredService<ApplicationDbContext>(), sp.GetRequiredService<IMediator>()));
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, Task<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetRequiredService<ApplicationDbContext>()));
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, ValueTask<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetRequiredService<ApplicationDbContext>()).AsTask());
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, IMediator, Task<T>> action)
    {
        return ExecuteScopeAsync(sp =>
            action(sp.GetRequiredService<ApplicationDbContext>(), sp.GetRequiredService<IMediator>()));
    }

    public Task InsertAsync<T>(params T[] entities) where T : class
    {
        return ExecuteDbContextAsync(db =>
        {
            foreach (var entity in entities) db.Set<T>().Add(entity);
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

    public Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4>(TEntity entity, TEntity2 entity2, TEntity3 entity3,
        TEntity4 entity4)
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
        where T : BaseEntity<TKey>
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

    private class TestApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _connectionString;

        public TestApplicationFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("ConnectionStrings:Database", _connectionString)
                });
            });

            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(
                    sp => new RequestExecutorProxy(
                        sp.GetRequiredService<IRequestExecutorResolver>(),
                        Schema.DefaultName));
            });
        }
    }
}