using System;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Auth.Exceptions;

public class IdentityException : Exception
{
    public IdentityResult Result { get; }
    public IdentityException(IdentityResult result)
        : base("An identity error has occurred.")
    {
        Result = result;
    }
}