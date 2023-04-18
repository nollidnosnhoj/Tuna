using System;
using Tuna.Shared.Models;

namespace Tuna.GraphQl.Features.Audios.Errors;

public class AudioNotUploadedError : IUserError
{
    public AudioNotUploadedError(AudioNotUploadedException ex)
    {
        Code = GetType().Name;
        Message = ex.Message;
    }

    public string Code { get; }
    public string Message { get; }
}

public class AudioNotUploadedException : Exception
{
    public AudioNotUploadedException() : base("Audio has not been uploaded.")
    {
    }
}