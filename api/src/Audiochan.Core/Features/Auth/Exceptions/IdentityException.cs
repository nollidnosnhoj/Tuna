using System;
using System.Collections.Generic;
using System.Linq;

namespace Audiochan.Core.Features.Auth.Exceptions;

public record IdentityError(string Code, string Message);

public class IdentityException : Exception
{
    public List<IdentityError> Errors { get; }
    public IdentityException(IEnumerable<IdentityError> errors)
        : base("An identity error has occurred.")
    {
        Errors = errors.ToList();
    }
}