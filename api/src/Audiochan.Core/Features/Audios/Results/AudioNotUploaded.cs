using System;

namespace Audiochan.Core.Features.Audios.Results;

public struct AudioNotUploaded
{
    public AudioNotUploaded(string uploadId)
    {
        UploadId = uploadId;
    }

    public string UploadId { get; }
}