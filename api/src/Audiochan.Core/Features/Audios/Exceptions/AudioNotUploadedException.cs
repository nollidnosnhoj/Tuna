using System;

namespace Audiochan.Core.Features.Audios.Exceptions;

public class AudioNotUploadedException : Exception
{
    public string UploadId { get; }
    public string FileName { get; }
    public AudioNotUploadedException(string uploadId, string fileName) : base($"Audio has not uploaded yet.")
    {
        UploadId = uploadId;
        FileName = fileName;
    }
}