using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;

namespace Audiochan.Core.Interfaces
{
    public interface IStorageService
    {
        Task DeleteBlobAsync(string container, string blobName, CancellationToken cancellationToken = default);
        Task SaveBlobAsync(string container, string blobName, Stream stream, bool overwrite = true,
            CancellationToken cancellationToken = default);

        Task<BlobDto> GetBlobAsync(string container, string blobName,
            CancellationToken cancellationToken = default);
    }
}
