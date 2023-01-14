using System.Net.Http;
using System.Net.Http.Headers;
using Audiochan.Tests.Common.Mocks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.Core.IntegrationTests.Extensions;

public static class WebApplicationFactoryExtensions
{
    public static WebApplicationFactory<T> WithAuthentication<T>(this WebApplicationFactory<T> factory) where T : class
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Configure<MockAuthenticationHandlerOptions>(o =>
                {
                    o.DefaultUserId = 1;
                    o.DefaultUserName = "testuser";
                });

                services.AddAuthentication(MockAuthenticationHandler.AuthenticationScheme)
                    .AddScheme<MockAuthenticationHandlerOptions, MockAuthenticationHandler>(
                        MockAuthenticationHandler.AuthenticationScheme, o =>
                        {

                        });
            });
        });
    }

    public static HttpClient CreateClientWithTestAuth<T>(this WebApplicationFactory<T> factory) where T : class
    {
        var client = factory.WithAuthentication().CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
        return client;
    }
}