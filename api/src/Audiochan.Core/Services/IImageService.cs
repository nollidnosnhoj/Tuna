using System.Threading.Tasks;

namespace Audiochan.Core.Services;

public interface IImageService
{
    Task<PrepareUploadResult> PrepareUploadAsync(string fileName, string contentType);
    Task DeleteImageAsync(string imageId);
}

public record PrepareUploadResult(string Url, string UploadId);