using System;

namespace Tuna.Application.Features.Audios.Exceptions;

public class AudioNotUploadedException : Exception
{
    public long AudioId { get; }
    
    public AudioNotUploadedException(long audioId)
        : base($"Audio with id: {audioId} has not been uploaded yet.")
    {
        AudioId = audioId;
    }
}