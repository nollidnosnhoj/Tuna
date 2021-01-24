using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Audiochan.Core.Interfaces
{
    public interface IImageService
    {
        Task<BlobDto> UploadAudioImage(IFormFile file, string audioId, CancellationToken cancellationToken = default);
        Task<BlobDto> UploadUserImage(IFormFile file, string userId, CancellationToken cancellationToken = default);
        Task RemoveAudioImages(string audioId, CancellationToken cancellationToken = default);
        Task RemoveUserImages(string userId, CancellationToken cancellationToken = default);
    }
}