using System;

namespace Audiochan.Core.Features.Users.Exceptions;

public class UserNotFoundException : Exception
{
    public long UserId { get; set; }

    public UserNotFoundException(long userId) : base($"User with id ({userId}) was not found.")
    {
        UserId = userId;
    }
}