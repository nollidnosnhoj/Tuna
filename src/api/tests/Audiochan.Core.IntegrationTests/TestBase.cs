using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Domain.Entities;
using Audiochan.Domain.Enums;
using Audiochan.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Audiochan.Core.IntegrationTests
{
    [Collection(nameof(TestFixture))]
    public class TestBase
    {
        protected readonly TestWebApplicationFactory Factory;

        public TestBase(TestFixture fixture)
        {
            Factory = fixture.Factory;
        }

        protected void ExecuteDbContext(Action<ApplicationDbContext> action)
        {
            using var scope = Factory.Services.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            action(db);
        }

        protected T ExecuteDbContext<T>(Func<ApplicationDbContext, T> action)
        {
            using var scope = Factory.Services.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return action(db);
        }
        
        protected async Task ExecuteDbContextAsync(Func<ApplicationDbContext, Task> action)
        {
            using var scope = Factory.Services.CreateScope();
            await using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await action(db);
        }

        protected async Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, Task<T>> action)
        {
            using var scope = Factory.Services.CreateScope();
            await using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await action(db);
        }
        
        protected async ValueTask ExecuteDbContextAsync(Func<ApplicationDbContext, ValueTask> action)
        {
            using var scope = Factory.Services.CreateScope();
            await using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await action(db);
        }

        protected async ValueTask<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, ValueTask<T>> action)
        {
            using var scope = Factory.Services.CreateScope();
            await using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await action(db);
        }

        protected void ExecuteScope(Action<IServiceProvider> action)
        {
            using var scope = Factory.Services.CreateScope();
            action(scope.ServiceProvider);
        }
        
        protected T ExecuteScope<T>(Func<IServiceProvider, T> action)
        {
            using var scope = Factory.Services.CreateScope();
            return action(scope.ServiceProvider);
        }
        
        protected async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using var scope = Factory.Services.CreateScope();
            await action(scope.ServiceProvider);
        }

        protected async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using var scope =  Factory.Services.CreateScope();
            var result = await action(scope.ServiceProvider);
            return result;
        }
        
        protected async ValueTask ExecuteScopeAsync(Func<IServiceProvider, ValueTask> action)
        {
            using var scope = Factory.Services.CreateScope();
            await action(scope.ServiceProvider);
        }

        protected async ValueTask<T> ExecuteScopeAsync<T>(Func<IServiceProvider, ValueTask<T>> action)
        {
            using var scope =  Factory.Services.CreateScope();
            var result = await action(scope.ServiceProvider);
            return result;
        }
        
        protected void InsertIntoDatabase<T>(params T[] entities) where T : class
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

        protected void InsertRangeIntoDatabase<T>(IEnumerable<T> entities) where T : class
        {
            ExecuteDbContext(db =>
            {
                db.Set<T>().AddRange(entities);
                db.SaveChanges();
            });
        }

        protected Task<(bool, TResponse?)> GetCache<TResponse>(string key)
        {
            return ExecuteScopeAsync(sp =>
            {
                var cache = sp.GetService<ICacheService>();
                return cache?.GetAsync<TResponse>(key) ?? throw new Exception("ICacheService was not registered.");
            });
        }

        protected Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetRequiredService<IMediator>();

                return mediator.Send(request);
            });
        }

        protected Task SendAsync(IRequest request)
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetRequiredService<IMediator>();

                return mediator.Send(request);
            });
        }

        protected async Task<(long, string)> RunAsDefaultUserAsync()
        {
            return await RunAsUserAsync("defaultuser", "Testing1234!");
        }
        
        // protected async Task<(long, string)> RunAsModeratorAsync()
        // {
        //     return await RunAsUserAsync("admin", "Administrator1234!", UserRole.Moderator);
        // }
        //
        // protected async Task<(long, string)> RunAsAdministratorAsync()
        // {
        //     return await RunAsUserAsync("admin", "Administrator1234!", UserRole.Admin);
        // }

        protected async Task<(long, string)> RunAsUserAsync(string userName = "", string password = "", UserRole role = UserRole.Regular)
        {
            using var scope =  Factory.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>()
                              ?? throw new Exception("No user manager");

            if (!string.IsNullOrEmpty(userName))
            {
                var user = await dbContext.Users.SingleOrDefaultAsync(u => u.UserName == userName);

                if (user != null)
                {
                    Factory.CurrentUserId = user.Id;
                    Factory.CurrentUserName = user.UserName;
                    return (Factory.CurrentUserId, Factory.CurrentUserName);
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
            
            Factory.CurrentUserId = newUser.Id;
            Factory.CurrentUserName = newUser.UserName;
            
            return (Factory.CurrentUserId, Factory.CurrentUserName);
        }

        protected DateTime GetCurrentTime()
        {
            var dtProvider = ExecuteScope(services => services.GetRequiredService<IDateTimeProvider>());
            return dtProvider.Now;
        }
    }
}