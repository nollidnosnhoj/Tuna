using System;
using System.Net.Http.Headers;
using Tuna.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Tuna.Infrastructure.Images.CloudflareImages;

internal static class CloudflareImageServiceCollectionExtensions
{
    internal static IServiceCollection AddCloudflareImages(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddRefitClient<ICloudflareImagesApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(configuration["Cloudflare:Images:ApiUrl"]!);
                c.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", configuration["Cloudflare:Images:ApiKey"]!);
            })
            .Services
            .AddTransient<IImageService, CloudflareImagesService>();
    }
}