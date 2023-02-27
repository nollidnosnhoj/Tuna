using System.Collections.Generic;
using Audiochan.Common.Models;

namespace Audiochan.API.Features.Users.Payloads;

public class UpdatePasswordPayload : Payload<IUserError>
{
    public UpdatePasswordPayload()
    {
    }

    public UpdatePasswordPayload(params IUserError[] errors) : base(errors)
    {
    }

    public UpdatePasswordPayload(IEnumerable<IUserError> errors, string? message = null) : base(errors, message)
    {
    }

    public UpdatePasswordPayload(string? message) : base(message)
    {
    }
}