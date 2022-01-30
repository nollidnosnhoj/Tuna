using Audiochan.Application.Commons.Exceptions;

namespace Audiochan.Application.Features.Audios.Exceptions;

public class UploadDoesNotExistException : BadRequestException
{
    public UploadDoesNotExistException() : base("Cannot find upload. Please upload and try again.")
    {
    }
}