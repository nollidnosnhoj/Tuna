using System.Collections.Generic;
using Audiochan.Common.Models;
using Audiochan.Core.Features.Upload.Models;

namespace Audiochan.API.Features.Audios.Payloads;

public class UpdatePicturePayload : Payload<UserError>
{
    public UpdatePicturePayload(ImageUploadResult result)
    {
        Result = result;
    }
    
    public UpdatePicturePayload(params UserError[] errors) : base(errors)
    {
    }

    public UpdatePicturePayload(IEnumerable<UserError> errors, string? message = null) : base(errors, message)
    {
    }

    public UpdatePicturePayload(string? message) : base(message)
    {
    }
    
    public ImageUploadResult? Result { get; }
}