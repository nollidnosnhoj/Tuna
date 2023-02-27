using System.Collections.Generic;
using Audiochan.Common.Models;

namespace Audiochan.API.Features.Users.Payloads;

public class UpdateUsernamePayload : Payload<IUserError>
{
    public UpdateUsernamePayload()
    {
    }

    public UpdateUsernamePayload(params IUserError[] errors) : base(errors)
    {
    }

    public UpdateUsernamePayload(IEnumerable<IUserError> errors, string? message = null) : base(errors, message)
    {
    }

    public UpdateUsernamePayload(string? message) : base(message)
    {
    }
}