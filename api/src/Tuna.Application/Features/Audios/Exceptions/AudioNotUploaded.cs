using System;

namespace Tuna.Application.Features.Audios.Exceptions;

public class AudioNotUploadedException : Exception
{
    public AudioNotUploadedException(long audioId)
        : base($"Audio with id: {audioId} has not been uploaded yet.")
    {
        AudioId = audioId;
    }

    public long AudioId { get; }
}