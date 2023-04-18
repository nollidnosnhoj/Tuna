using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tuna.Application.Services;

public interface IImageService
{
    Task<PrepareUploadResult> PrepareUploadAsync(Dictionary<string, string>? metadata);
    Task DeleteImageAsync(string imageId);
    string GetImageUrl(string imageId, string? variant);
}

public record PrepareUploadResult(string Url, string UploadId);