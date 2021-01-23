using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Audiochan.Core.Interfaces
{
    public interface IImageService
    {
        Task<BlobDto> UploadArtwork(string imageData, string container, string name,
            CancellationToken cancellationToken = default);

        Task<BlobDto> UploadArtwork(IFormFile file, string container, string name,
            CancellationToken cancellationToken = default);

        Task DeleteArtworkAndThumbnails(string container, string name,
            CancellationToken cancellationToken = default);
    }
}