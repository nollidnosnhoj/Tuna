using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Models.Responses;

namespace Audiochan.Core.Interfaces
{
    public interface IImageService
    {
        Task<SaveBlobResponse> UploadImage(string data, string container, string blobName,
            CancellationToken cancellationToken = default);
    }
}