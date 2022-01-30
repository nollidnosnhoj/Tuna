using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Audiochan.API;
using Audiochan.Core.Commons.Extensions;

using Audiochan.Core.Commons.Services;
using Audiochan.Core.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.Domain.Enums;
using Audiochan.Infrastructure.Persistence;
using Audiochan.Tests.Common.Mocks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Npgsql;
using NUnit.Framework;
using Respawn;
#pragma warning disable 8618

namespace Audiochan.Core.IntegrationTests
{
    [SetUpFixture]
    public class TestFixture
    {
        private static IConfigurationRoot _configuration;
        private static IWebHostEnvironment _env;
        private static IServiceScopeFactory _scopeFactory;
        private static Checkpoint _checkpoint;

        private static ClaimsPrincipal? _user;

        [OneTimeSetUp]
        public async Task RunBeforeAnyTests()
        {
            // Create database container for integration tests
            var dockerSqlPort = await DockerDatabaseUtilities.EnsureDockerStartedAsync();
            var dockerConnectionString = DockerDatabaseUtilities.GetSqlConnectionString(dockerSqlPort);

            // Build configuration, including the connection string from the database container
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:Database", dockerConnectionString }
                })
                .AddEnvironmentVariables();

            _configuration = builder.Build();
            _env = Mock.Of<IWebHostEnvironment>();

            var startup = new Startup(_configuration, _env);
            var services = new ServiceCollection();
            services.AddLogging();
            startup.ConfigureServices(services);

            #region Register mocks

            var descriptorCurrentUserService = services.FirstOrDefault(d => d.ServiceType == typeof(ICurrentUserService));
            services.Remove(descriptorCurrentUserService!);
            services.AddTransient(_ => CurrentUserServiceMock.Create(_user).Object);
                
            var descriptorDateTimeProvider = services.FirstOrDefault(d => d.ServiceType == typeof(IDateTimeProvider));
            services.Remove(descriptorDateTimeProvider!);
            services.AddTransient(_ => DateTimeProviderMock.Create(DateTime.UtcNow).Object);

            var descriptorStorageService = services.FirstOrDefault(d => d.ServiceType == typeof(IStorageService));
            services.Remove(descriptorStorageService!);
            services.AddTransient(_ => StorageServiceMock.Create().Object);

            #endregion
            
            _scopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>() 
                            ?? throw new InvalidOperationException();
            
            _checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] { "__EFMigrationsHistory" },
                DbAdapter = DbAdapter.Postgres
            };
            
            EnsureDatabase();
        }
        
        public static void ExecuteDbContext(Action<ApplicationDbContext> action)
        {
            using var scope = _scopeFactory.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            action(db);
        }

        public static T ExecuteDbContext<T>(Func<ApplicationDbContext, T> action)
        {
            using var scope = _scopeFactory.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return action(db);
        }
        
        public static async Task ExecuteDbContextAsync(Func<ApplicationDbContext, Task> action)
        {
            using var scope = _scopeFactory.CreateScope();
            await using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await action(db);
        }

        public static async Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, Task<T>> action)
        {
            using var scope = _scopeFactory.CreateScope();
            await using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await action(db);
        }
        
        public static async ValueTask ExecuteDbContextAsync(Func<ApplicationDbContext, ValueTask> action)
        {
            using var scope = _scopeFactory.CreateScope();
            await using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await action(db);
        }

        public static async ValueTask<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, ValueTask<T>> action)
        {
            using var scope = _scopeFactory.CreateScope();
            await using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await action(db);
        }

        public static void ExecuteScope(Action<IServiceProvider> action)
        {
            using var scope = _scopeFactory.CreateScope();
            action(scope.ServiceProvider);
        }
        
        public static T ExecuteScope<T>(Func<IServiceProvider, T> action)
        {
            using var scope = _scopeFactory.CreateScope();
            return action(scope.ServiceProvider);
        }
        
        public static async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using var scope = _scopeFactory.CreateScope();
            await action(scope.ServiceProvider);
        }

        public static async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using var scope =  _scopeFactory.CreateScope();
            var result = await action(scope.ServiceProvider);
            return result;
        }
        
        public static async ValueTask ExecuteScopeAsync(Func<IServiceProvider, ValueTask> action)
        {
            using var scope = _scopeFactory.CreateScope();
            await action(scope.ServiceProvider);
        }

        public static async ValueTask<T> ExecuteScopeAsync<T>(Func<IServiceProvider, ValueTask<T>> action)
        {
            using var scope =  _scopeFactory.CreateScope();
            var result = await action(scope.ServiceProvider);
            return result;
        }
        
        public static void InsertIntoDatabase<T>(params T[] entities) where T : class
        {
            ExecuteDbContext(db =>
            {
                foreach (var entity in entities)
                {
                    db.Set<T>().Add(entity);
                }

                db.SaveChanges();
            });
        }

        public static void InsertRangeIntoDatabase<T>(IEnumerable<T> entities) where T : class
        {
            ExecuteDbContext(db =>
            {
                db.Set<T>().AddRange(entities);
                db.SaveChanges();
            });
        }

        public static Task<TResponse?> GetCache<TResponse>(string key)
        {
            return ExecuteScopeAsync(sp =>
            {
                var cache = sp.GetRequiredService<IDistributedCache>();
                return cache.GetAsync<TResponse>(key);
            });
        }

        public static Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetRequiredService<IMediator>();

                return mediator.Send(request);
            });
        }

        public static Task SendAsync(IRequest request)
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetRequiredService<IMediator>();

                return mediator.Send(request);
            });
        }

        public static void ClearCurrentUser()
        {
            _user = null;
        }

        public static async Task<ClaimsPrincipal> RunAsDefaultUserAsync()
        {
            return await RunAsUserAsync("defaultuser", "Testing1234!");
        }

        public static async Task<ClaimsPrincipal> RunAsUserAsync(string userName = "", string password = "", UserRole role = UserRole.Regular)
        {
            using var scope =  _scopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>()
                              ?? throw new Exception("No user manager");

            if (!string.IsNullOrEmpty(userName))
            {
                var user = await dbContext.Users.SingleOrDefaultAsync(u => u.UserName == userName);

                if (user != null)
                {
                    return CurrentUserServiceMock.CreateMockPrincipal(user.Id, user.UserName);
                }
            }
            else
            {
                userName = $"user{DateTime.Now.Ticks}";
            }

            if (string.IsNullOrEmpty(password))
            {
                password = Guid.NewGuid().ToString("N");
            }
            
            var pwHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            password = pwHasher.Hash(password);
            
            var newUser = new User(userName, userName + "@localhost", password, role);

            await dbContext.Users.AddAsync(newUser);
            await dbContext.SaveChangesAsync();

            _user = CurrentUserServiceMock.CreateMockPrincipal(newUser.Id, newUser.UserName);
            
            return _user;
        }

        public static async Task ResetState()
        {
            await using var conn = new NpgsqlConnection(_configuration.GetConnectionString("Database"));
            await conn.OpenAsync();
            await _checkpoint.Reset(conn);
        }

        private static void EnsureDatabase()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            context?.Database.Migrate();
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            
        }
    }
}