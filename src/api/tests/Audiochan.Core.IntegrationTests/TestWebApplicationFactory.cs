using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Audiochan.API;
using Audiochan.Core.Interfaces;
using Audiochan.Tests.Common.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Audiochan.Core.IntegrationTests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Startup>
    {
        public long CurrentUserId { get; set; }
        public string? CurrentUserName { get; set; }
        
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
                var descriptorCurrentUserService = services.FirstOrDefault(d => d.ServiceType == typeof(ICurrentUserService));
                services.Remove(descriptorCurrentUserService!);
                services.AddTransient(_ => CurrentUserServiceMock.Create(CurrentUserId, CurrentUserName).Object);
                
                var descriptorDateTimeProvider = services.FirstOrDefault(d => d.ServiceType == typeof(IDateTimeProvider));
                services.Remove(descriptorDateTimeProvider!);
                services.AddTransient(_ => DateTimeProviderMock.Create(DateTime.UtcNow).Object);

                var descriptorStorageService = services.FirstOrDefault(d => d.ServiceType == typeof(IStorageService));
                services.Remove(descriptorStorageService!);
                services.AddTransient(_ => StorageServiceMock.Create().Object);
            });

            base.ConfigureWebHost(builder);
        }
    }
}