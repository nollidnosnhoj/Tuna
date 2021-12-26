using Audiochan.Application.Commons.Exceptions;

namespace Audiochan.Application.Features.Audios.Exceptions;

public class UploadDoesNotExistException : BadRequestException
{
    public string UploadId { get; }
    
    public UploadDoesNotExistException(string uploadId) : base("Cannot find upload. Please upload and try again.")
    {
        UploadId = uploadId;
    }
}