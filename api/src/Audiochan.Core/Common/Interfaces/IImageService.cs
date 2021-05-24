using System.Threading;
using System.Threading.Tasks;

namespace Audiochan.Core.Common.Interfaces
{
    public interface IImageService
    {
        Task UploadImage(string data, string container, string blobName,
            CancellationToken cancellationToken = default);
    }
}