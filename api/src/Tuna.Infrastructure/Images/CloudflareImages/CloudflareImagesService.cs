using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Tuna.Application.Exceptions;
using Tuna.Application.Services;
using Tuna.Infrastructure.Images.CloudflareImages.Models;

namespace Tuna.Infrastructure.Images.CloudflareImages;

public class CloudflareImagesService : IImageService
{
    private readonly ICloudflareImagesApi _api;
    private readonly string _serveUrl;
    
    public CloudflareImagesService(ICloudflareImagesApi api, IConfiguration configuration)
    {
        _api = api;
        _serveUrl = configuration.GetValue<string>("Cloudflare:Images:ServeUrl")
            ?? throw new ArgumentNullException(nameof(configuration), "Missing serve url configuration for Cloudflare Images.");
    }
    
    public async Task<PrepareUploadResult> PrepareUploadAsync(Dictionary<string, string>? metadata)
    {
        var request = new DirectUploadRequest
        {
            RequireSignedUrls = true,
            Metadata = metadata is not null
                ? JsonSerializer.Serialize(metadata)
                : null
        };
        
        var response = await _api.DirectUploadAsync(request);

        if (response.Result is not null)
        {
            return new PrepareUploadResult(response.Result.UploadUrl, response.Result.Id);
        }

        if (response.Errors.Count > 0)
        {
            throw new ImageServiceAggregateException("Failed to prepare image upload.", response.Errors);
        }
        
        throw new ImageServiceException("Failed to prepare image upload.");
    }

    public async Task DeleteImageAsync(string imageId)
    {
        var response = await _api.DeleteImageAsync(imageId);
        if (response.Success) return;
        if (response.Errors.Count > 0)
        {
            throw new ImageServiceAggregateException("Unable to remove image from Cloudflare Images.", response.Errors);
        }

        throw new ImageServiceException("Unable to remove image from Cloudflare Images for unknown reasons.");
    }

    public string GetImageUrl(string imageId, string? variant = "public")
    {
        return $"{_serveUrl}/{imageId}/{variant}";
    }
}