using System.Collections.Generic;
using Audiochan.Common.Models;

namespace Audiochan.API.Features.Audios.Payloads;

public class RemovePicturePayload : Payload<UserError>
{
    public RemovePicturePayload()
    {
    }

    public RemovePicturePayload(params UserError[] errors) : base(errors)
    {
    }

    public RemovePicturePayload(IEnumerable<UserError> errors, string? message = null) : base(errors, message)
    {
    }

    public RemovePicturePayload(string? message) : base(message)
    {
    }
}