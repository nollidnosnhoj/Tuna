using System.Threading;
using System.Threading.Tasks;

namespace Audiochan.Core.Services
{
    public interface IImageProcessingService
    {
        Task UploadImage(string data, string container, string blobName,
            CancellationToken cancellationToken = default);
    }
}