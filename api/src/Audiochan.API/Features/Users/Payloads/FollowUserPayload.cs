using System.Collections.Generic;
using Audiochan.Common.Models;

namespace Audiochan.API.Features.Users.Payloads;

public class FollowUserPayload : Payload<IUserError>
{
    public FollowUserPayload(bool isFollowing)
    {
        IsFollowing = isFollowing;
    }

    public FollowUserPayload(params IUserError[] errors) : base(errors)
    {
    }

    public FollowUserPayload(IEnumerable<IUserError> errors, string? message = null) : base(errors, message)
    {
    }

    public FollowUserPayload(string? message) : base(message)
    {
    }
    
    public bool? IsFollowing { get; }
}