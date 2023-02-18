using System;

namespace Audiochan.Core.Features.Users.Exceptions;

public class DuplicateEmailException : Exception
{
    public string Email { get; }
    public DuplicateEmailException(string email)
        : base($"\\\"{email}\\\" already exists.")
    {
        Email = email;
    }
}