using System.Collections.Generic;
using Audiochan.Common.Models;

namespace Audiochan.API.Features.Users.Payloads;

public class UpdateEmailPayload : Payload<IUserError>
{
    public UpdateEmailPayload()
    {
    }

    public UpdateEmailPayload(params IUserError[] errors) : base(errors)
    {
    }

    public UpdateEmailPayload(IEnumerable<IUserError> errors, string? message = null) : base(errors, message)
    {
    }

    public UpdateEmailPayload(string? message) : base(message)
    {
    }
}