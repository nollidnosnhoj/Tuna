﻿using System;

namespace Tuna.Application.Features.Users.Exceptions;

public class FollowingException : Exception
{
    public FollowingException(long targetId, string? message = null)
        : base(message ?? $"An error has occurred while trying to follow user with id: {targetId}.")
    {
        TargetId = targetId;
    }

    public long TargetId { get; }
}