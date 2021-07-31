using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
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
        
        protected void Insert<T>(params T[] entities) where T : class
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

        protected void InsertAsync<T>(T entity) where T : class
        {
            ExecuteDbContext(db =>
            {
                db.Set<T>().Add(entity);
                db.SaveChanges();
            });
        }

        protected void InsertRange<T>(IEnumerable<T> entities) where T : class
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

        protected async Task<(string, string)> RunAsDefaultUserAsync()
        {
            return await RunAsUserAsync("defaultuser", "Testing1234!", Array.Empty<string>());
        }

        protected async Task<(string, string)> RunAsAdministratorAsync()
        {
            return await RunAsUserAsync("admin", "Administrator1234!", new[] {UserRoleConstants.Admin});
        }

        protected async Task<(string, string)> RunAsUserAsync(string userName, string password = "", string[]? roles = null)
        {
            using var scope =  Factory.Services.CreateScope();

            var userManager = scope.ServiceProvider.GetService<UserManager<User>>()
                              ?? throw new Exception("No user manager");

            var user = await userManager.FindByNameAsync(userName);

            if (user != null)
            {
                Factory.CurrentUserId = user.Id;
                Factory.CurrentUserName = user.UserName;
                return (Factory.CurrentUserId, Factory.CurrentUserName);
            }
            
            user = new User
            {
                UserName = userName,
                Email = userName + "@localhost",
                DisplayName = userName,
                Joined = Factory.CurrentTime
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
                Factory.CurrentUserId = user.Id;
                Factory.CurrentUserName = user.UserName;
                return (Factory.CurrentUserId, Factory.CurrentUserName);
            }

            var errors = string.Join(Environment.NewLine, result.ToResult().Errors!);

            throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
        }
    }
}