using System.Threading.Tasks;
using Refit;
using Tuna.Infrastructure.Images.CloudflareImages.Models;

namespace Tuna.Infrastructure.Images.CloudflareImages;

[Headers("Authorization: Bearer")]
public interface ICloudflareImagesApi
{
    [Post("/images/v2/direct_upload")]
    Task<CloudflareResponse<DirectUploadResult>> DirectUploadAsync([Body(BodySerializationMethod.UrlEncoded)] DirectUploadRequest request);
    
    [Delete("/images/v1/{imageId}")]
    Task<CloudflareResponse> DeleteImageAsync(string imageId);
}