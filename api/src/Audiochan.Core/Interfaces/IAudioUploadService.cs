using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Audiochan.Core.Interfaces
{
    public interface IAudioUploadService
    {
        Task<AudioUploadResult> UploadAudio(IFormFile audioFile, string id, CancellationToken cancellationToken = default);
        Task DeleteAudio(string id, string fileExt, CancellationToken cancellationToken = default);
    }
}