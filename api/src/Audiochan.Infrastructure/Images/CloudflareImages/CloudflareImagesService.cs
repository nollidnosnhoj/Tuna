using System.Threading.Tasks;
using Audiochan.Core.Exceptions;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Images.CloudflareImages.Models;

namespace Audiochan.Infrastructure.Images.CloudflareImages;

public class CloudflareImagesService : IImageService
{
    private readonly ICloudflareImagesApi _api;
    
    public CloudflareImagesService(ICloudflareImagesApi api)
    {
        _api = api;
    }
    
    public async Task<PrepareUploadResult> PrepareUploadAsync(string fileName, string contentType)
    {
        var request = new DirectUploadRequest
        {
            RequireSignedUrls = true,
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
}