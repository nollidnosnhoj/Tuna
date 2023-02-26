using System;
using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Features.Auth.Models;

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