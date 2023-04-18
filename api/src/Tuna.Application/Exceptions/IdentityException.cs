using System;
using Microsoft.AspNetCore.Identity;

namespace Tuna.Application.Exceptions;

public class IdentityException : Exception
{
    public IdentityException(IdentityError error) : base(error.Description)
    {
        Error = error;
    }

    public IdentityError Error { get; }
}