using System;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Exceptions;

public class IdentityException : Exception
{
    public IdentityError Error { get; }

    public IdentityException(IdentityError error) : base(error.Description)
    {
        Error = error;
    }
}