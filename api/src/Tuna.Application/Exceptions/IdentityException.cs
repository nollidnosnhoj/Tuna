using System;
using Microsoft.AspNetCore.Identity;

namespace Tuna.Application.Exceptions;

public class IdentityException : Exception
{
    public IdentityError Error { get; }

    public IdentityException(IdentityError error) : base(error.Description)
    {
        Error = error;
    }
}