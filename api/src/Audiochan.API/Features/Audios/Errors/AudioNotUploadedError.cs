using Audiochan.Common.Models;
using Audiochan.Core.Features.Audios.Exceptions;

namespace Audiochan.API.Features.Audios.Errors;

public class AudioNotUploadedError : IUserError
{
    public AudioNotUploadedError(AudioNotUploadedException exception)
    {
        UploadId = exception.UploadId;
    }

    public string UploadId { get; }
    public string Code => GetType().Name;
    public string Message => "Audio has not been uploaded.";
}