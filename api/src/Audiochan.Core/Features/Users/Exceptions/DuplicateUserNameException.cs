using System;

namespace Audiochan.Core.Features.Users.Exceptions;

public class DuplicateUserNameException : Exception
{
    public string UserName { get; }

    public DuplicateUserNameException(string userName)
        : base($"\\\"{userName}\\\" already exists.")
    {
        UserName = userName;
    }
}