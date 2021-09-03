using System.Threading;
using System.Threading.Tasks;

namespace Audiochan.Core.Interfaces
{
    public interface IImageUploadService
    {
        Task UploadImage(string data, string container, string blobName, CancellationToken cancellationToken = default);
        Task RemoveImage(string container, string blobName, CancellationToken cancellationToken = default);
        bool ValidateImageSize(string base64, int min, int max, int? minHeight = null, int? maxHeight = null);
    }
}