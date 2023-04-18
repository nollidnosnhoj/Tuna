using System;

namespace Tuna.Application.Features.Audios.Errors;

public struct AudioNotUploaded
{
    public AudioNotUploaded(string uploadId)
    {
        UploadId = uploadId;
    }

    public string UploadId { get; }
}