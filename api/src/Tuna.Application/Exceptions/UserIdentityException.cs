using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Tuna.Application.Exceptions;

public class UserIdentityException : Exception
{
    public IReadOnlyList<IdentityError> Errors { get; }

    public UserIdentityException(IEnumerable<IdentityError> errors)
        : base("An error occurred while processing an action in identity service.")
    {
        Errors = errors.ToList();
    }
}