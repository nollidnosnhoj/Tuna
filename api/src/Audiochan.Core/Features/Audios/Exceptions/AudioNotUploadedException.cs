using System;

namespace Audiochan.Core.Features.Audios.Exceptions;

public class AudioNotUploadedException : Exception
{
    public AudioNotUploadedException() : base($"Audio has not uploaded yet.")
    {
        
    }
}