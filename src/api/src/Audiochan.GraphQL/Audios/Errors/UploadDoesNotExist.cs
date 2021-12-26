using Audiochan.Application.Features.Audios.Exceptions;
using Audiochan.GraphQL.Common.Errors;

namespace Audiochan.GraphQL.Audios.Errors;

public class UploadDoesNotExist : GraphQlError
{
    public string UploadId { get; }

    public UploadDoesNotExist(UploadDoesNotExistException ex) : base(ex.Message)
    {
        UploadId = ex.UploadId;
    }
}