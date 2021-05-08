using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.API;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Entities;
using Audiochan.Infrastructure.Persistence;
using Audiochan.UnitTests.Mocks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using Npgsql;
using Respawn;
using Xunit;

namespace Audiochan.IntegrationTests
{
    [CollectionDefinition(nameof(SliceFixture))]
    public class SliceFixtureCollection : ICollectionFixture<SliceFixture>
    {
    }

    public class SliceFixture : IAsyncLifetime
    {
        private readonly Checkpoint _checkpoint;
        private readonly IConfiguration _configuration;
        private static IServiceScopeFactory _scopeFactory;
        private readonly WebApplicationFactory<Startup> _factory;
        private static string _currentUserId;
        private static string _currentUsername;

        public SliceFixture()
        {
            _factory = new AudiochanTestApplicationFactory();
            _configuration = _factory.Services.GetRequiredService<IConfiguration>();
            _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
            _checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] {"__EFMigrationsHistory"},
                SchemasToInclude = new[] {"public"},
                DbAdapter = DbAdapter.Postgres
            };
            EnsureDatabase();
        }

        public class AudiochanTestApplicationFactory : WebApplicationFactory<Startup>
        {
            private readonly string _connectionString =
                "Server=localhost;Port=5433;Database=audiochan_test;Username=postgres;Password=pokemon123;";

            protected override IHost CreateHost(IHostBuilder builder)
            {
                builder.UseContentRoot(Directory.GetCurrentDirectory());
                return base.CreateHost(builder);
            }

            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.ConfigureAppConfiguration((_, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"ConnectionStrings:Database", _connectionString}
                    });
                });

                builder.ConfigureServices(services =>
                {
                    ReplaceCurrentUserService(services);
                    ReplaceStorageService(services);
                });

                base.ConfigureWebHost(builder);
            }
        }

        public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                dbContext.BeginTransaction();
                await action(scope.ServiceProvider);
                dbContext.CommitTransaction();
            }
            catch (Exception)
            {
                dbContext.RollbackTransaction();
                throw;
            }
        }

        public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                dbContext.BeginTransaction();
                var result = await action(scope.ServiceProvider);
                dbContext.CommitTransaction();
                return result;
            }
            catch (Exception)
            {
                dbContext.RollbackTransaction();
                throw;
            }
        }

        public async Task<(string, string)> RunAsDefaultUserAsync()
        {
            return await RunAsUserAsync("testuser", "Testing1234!", Array.Empty<string>());
        }

        public async Task<(string, string)> RunAsAdministratorAsync()
        {
            return await RunAsUserAsync("admin", "Administrator1234!", new[] {UserRoleConstants.Admin});
        }

        public async Task<(string, string)> RunAsUserAsync(string userName, string password, string[] roles)
        {
            using var scope = _scopeFactory.CreateScope();

            var userManager = scope.ServiceProvider.GetService<UserManager<User>>()
                              ?? throw new Exception("No user manager");

            var user = await userManager.FindByNameAsync(userName);

            if (user != null)
            {
                _currentUserId = user.Id;
                _currentUsername = user.UserName;
                return (_currentUserId, _currentUsername);
            }

            user = new User
            {
                UserName = userName,
                Email = userName + "@localhost",
                DisplayName = userName,
                Joined = Instant.FromDateTimeUtc(DateTime.UtcNow)
            };

            var result = await userManager.CreateAsync(user, password);

            if (roles.Any())
            {
                var roleManager = scope.ServiceProvider.GetService<RoleManager<Role>>()
                                  ?? throw new Exception("No role manager");

                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(new Role(role));
                }

                await userManager.AddToRolesAsync(user, roles);
            }

            if (result.Succeeded)
            {
                _currentUserId = user.Id;
                _currentUsername = user.UserName;
                return (_currentUserId, _currentUsername);
            }

            var errors = string.Join(Environment.NewLine, result.ToResult().Errors);

            throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
        }

        public Task ExecuteDbContextAsync(Func<ApplicationDbContext, Task> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<ApplicationDbContext>()));

        public Task ExecuteDbContextAsync(Func<ApplicationDbContext, ValueTask> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<ApplicationDbContext>()).AsTask());

        public Task ExecuteDbContextAsync(Func<ApplicationDbContext, IMediator, Task> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<ApplicationDbContext>(), sp.GetService<IMediator>()));

        public Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, Task<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<ApplicationDbContext>()));

        public Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, ValueTask<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<ApplicationDbContext>()).AsTask());

        public Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, IMediator, Task<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<ApplicationDbContext>(), sp.GetService<IMediator>()));

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

        public Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4>(TEntity entity, TEntity2 entity2,
            TEntity3 entity3, TEntity4 entity4)
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

        public Task<T> FindAsync<T, TKey>(TKey id)
            where T : class
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
            await using (var conn = new NpgsqlConnection(_configuration.GetConnectionString("Database")))
            {
                await conn.OpenAsync();
                await _checkpoint.Reset(conn);
            }
            
            _currentUserId = null;
        }

        public Task DisposeAsync()
        {
            _factory?.Dispose();
            return Task.CompletedTask;
        }

        private static void ReplaceCurrentUserService(IServiceCollection services)
        {
            var currentUserServiceDescriptor = services.FirstOrDefault(d =>
                d.ServiceType == typeof(ICurrentUserService));

            services.Remove(currentUserServiceDescriptor);

            services.AddTransient(_ => CurrentUserServiceMock.Create(_currentUserId, _currentUsername).Object);
        }

        private static void ReplaceStorageService(IServiceCollection services)
        {
            var storageServiceDescriptor = services.FirstOrDefault(d =>
                d.ServiceType == typeof(IStorageService));

            services.Remove(storageServiceDescriptor);

            services.AddTransient(_ => StorageServiceMock.Create().Object);
        }

        private static void EnsureDatabase()
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

            context.Database.Migrate();
        }
    }
}