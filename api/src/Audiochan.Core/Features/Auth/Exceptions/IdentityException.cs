using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Auth.Exceptions;

public class IdentityException : Exception
{
    public IReadOnlyList<IdentityError> Errors { get; }

    public IdentityException(IEnumerable<IdentityError> errors)
    {
        Errors = errors.ToList();
    }
}