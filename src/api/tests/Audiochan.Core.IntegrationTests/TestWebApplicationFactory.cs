using System;
using System.Collections.Generic;
using System.IO;
using Audiochan.API;
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
        public string? CurrentUserId { get; set; }
        public string? CurrentUserName { get; set; }
        public DateTime CurrentTime { get; set; }

        public TestWebApplicationFactory()
        {
            CurrentTime = DateTime.UtcNow;
        }
        
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
                services.AddTransient(_ => CurrentUserServiceMock.Create(CurrentUserId, CurrentUserName).Object);
                services.AddTransient(_ => DateTimeProviderMock.Create(CurrentTime).Object);
                services.AddTransient(_ => StorageServiceMock.Create().Object);
            });

            base.ConfigureWebHost(builder);
        }
    }
}