using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;

namespace Audiochan.Core.Interfaces
{
    public interface IUploadService
    {
        GetUploadAudioUrlResponse GetUploadAudioUrl(GetUploadAudioUrlRequest request);
    }
}