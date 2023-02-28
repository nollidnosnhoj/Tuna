using System;
using Audiochan.Common.Models;

namespace Audiochan.API.Features.Audios.Errors;

public class AudioNotUploadedError : IUserError
{
    public string Code => GetType().Name;
    public string Message => "Audio has not been uploaded.";
}

public class AudioNotUploadedException : Exception
{
    public AudioNotUploadedException() : base("Audio has not been uploaded.")
    {
        
    }
}