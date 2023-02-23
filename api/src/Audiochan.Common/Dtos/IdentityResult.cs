﻿using Audiochan.Common.Exceptions;

namespace Audiochan.Common.Dtos;

public record NewUserIdentityResult(bool IsSuccess, string IdentityId, IEnumerable<IdentityError> Errors)
    : IdentityResult(IsSuccess, Errors);

public record IdentityResult(bool IsSuccess, IEnumerable<IdentityError> Errors)
{
    public static IdentityResult Succeed()
    {
        return new IdentityResult(true, Array.Empty<IdentityError>());
    }

    public static IdentityResult Error(IEnumerable<IdentityError> errors)
    {
        return new IdentityResult(false, errors);
    }

    public void EnsureSuccessfulResult()
    {
        if (!IsSuccess)
        {
            throw new IdentityException(Errors);
        }
    }
}