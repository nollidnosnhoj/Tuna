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
using Audiochan.Core.Interfaces;
using Audiochan.Tests.Common.Mocks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Respawn;
using Xunit;

namespace Audiochan.Core.IntegrationTests
{
    [CollectionDefinition(nameof(SliceFixture))]
    public class SliceFixtureCollection : ICollectionFixture<SliceFixture>
    {
    }

    public class SliceFixture : IAsyncLifetime
    {
        private readonly IConfiguration _configuration;
        private static IServiceScopeFactory _scopeFactory = null!;
        private readonly WebApplicationFactory<Startup> _factory;
        private static string? _currentUserId;
        private static string? _currentUsername;
        private static DateTime _currentTime;

        public SliceFixture()
        {
            _currentTime = DateTime.UtcNow;
            _factory = new AudiochanTestApplicationFactory();
            _configuration = _factory.Services.GetRequiredService<IConfiguration>()!;
            _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>()!;
            EnsureDatabase();
        }

        public class AudiochanTestApplicationFactory : WebApplicationFactory<Startup>
        {
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
                        {"ConnectionStrings:Database", "Server=localhost;Port=5433;Database=audiochan_test;Username=postgres;Password=pokemon123;"}
                    });
                });

                builder.ConfigureServices(services =>
                {
                    ReplaceCurrentUserService(services);
                    ReplaceDateTimeProvider(services);
                    ReplaceStorageService(services);
                });

                base.ConfigureWebHost(builder);
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
            var result = await action(scope.ServiceProvider);
            return result;
        }

        public async Task<(string, string)> RunAsDefaultUserAsync()
        {
            return await RunAsUserAsync("defaultuser", "Testing1234!", Array.Empty<string>());
        }

        public async Task<(string, string)> RunAsAdministratorAsync()
        {
            return await RunAsUserAsync("admin", "Administrator1234!", new[] {UserRoleConstants.Admin});
        }

        public async Task<(string, string)> RunAsUserAsync(string userName, string password = "", string[]? roles = null)
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
                Joined = _currentTime
            };
            
            if (string.IsNullOrEmpty(password))
                password = Guid.NewGuid().ToString("N");


            var result = await userManager.CreateAsync(user, password);

            roles ??= Array.Empty<string>();
            
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

            var errors = string.Join(Environment.NewLine, result.ToResult().Errors!);

            throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
        }

        public DateTime SetCurrentTime(DateTime nowDateTime)
        {
            _currentTime = nowDateTime;
            return nowDateTime;
        }

        public Task ExecuteDbContextAsync(Func<ApplicationDbContext, Task> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<ApplicationDbContext>()!));

        public Task ExecuteDbContextAsync(Func<ApplicationDbContext, ValueTask> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<ApplicationDbContext>()!).AsTask());

        public Task ExecuteDbContextAsync(Func<ApplicationDbContext, IMediator, Task> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<ApplicationDbContext>()!, sp.GetService<IMediator>()!));

        public Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, Task<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<ApplicationDbContext>()!));

        public Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, ValueTask<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<ApplicationDbContext>()!).AsTask());

        public Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, IMediator, Task<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<ApplicationDbContext>()!, sp.GetService<IMediator>()!));

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

        public Task InsertRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            return ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().AddRange(entities);
                return db.SaveChangesAsync();
            });
        }

        public Task<(bool, TResponse?)> GetCache<TResponse>(string key)
        {
            return ExecuteScopeAsync(sp =>
            {
                var cache = sp.GetService<ICacheService>();
                return cache?.GetAsync<TResponse>(key) ?? throw new Exception("ICacheService was not registered.");
            });
        }

        public Task<T> FindAsync<T, TKey>(TKey id) where T : class
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
            var connString = _configuration.GetConnectionString("Database");
            try
            {
                await using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    var cp = new Checkpoint
                    {
                        TablesToIgnore = new[] {"__EFMigrationsHistory"},
                        SchemasToInclude = new[] {"public"},
                        DbAdapter = DbAdapter.Postgres
                    };
                    await cp.Reset(conn);
                }

                _currentUserId = null;
            }
            catch
            {
                
            }
        }

        public Task DisposeAsync()
        {
            _factory.Dispose();
            return Task.CompletedTask;
        }

        private static void ReplaceDateTimeProvider(IServiceCollection services)
        {
            var clockDescriptor = services.FirstOrDefault(d =>
                d.ServiceType == typeof(IDateTimeProvider));

            if (clockDescriptor is not null)
                services.Remove(clockDescriptor);

            services.AddTransient(_ => DateTimeProviderMock.Create(_currentTime).Object);
        }

        private static void ReplaceCurrentUserService(IServiceCollection services)
        {
            var currentUserServiceDescriptor = services.FirstOrDefault(d =>
                d.ServiceType == typeof(ICurrentUserService));

            if (currentUserServiceDescriptor is not null)
                services.Remove(currentUserServiceDescriptor);

            services.AddTransient(_ => CurrentUserServiceMock.Create(_currentUserId, _currentUsername).Object);
        }

        private static void ReplaceStorageService(IServiceCollection services)
        {
            var storageServiceDescriptor = services.FirstOrDefault(d =>
                d.ServiceType == typeof(IStorageService));

            if (storageServiceDescriptor is not null)
                services.Remove(storageServiceDescriptor);

            services.AddTransient(_ => StorageServiceMock.Create().Object);
        }

        private static void EnsureDatabase()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            context?.Database.Migrate();
        }
    }
}