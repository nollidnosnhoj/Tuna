using System.Collections.Generic;
using Audiochan.Common.Models;
using Audiochan.Core.Features.Users.Models;

namespace Audiochan.API.Features.Users.Payloads;

public class UserPayload : Payload<UserError>
{
    public UserPayload(UserViewModel user)
    {
        User = user;
    }

    public UserPayload(params UserError[] errors) : base(errors)
    {
    }

    public UserPayload(IEnumerable<UserError> errors, string? message = null) : base(errors, message)
    {
    }

    public UserPayload(string? message) : base(message)
    {
    }
    
    public UserViewModel? User { get; }
}