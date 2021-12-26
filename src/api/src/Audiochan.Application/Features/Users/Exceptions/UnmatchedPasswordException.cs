using Audiochan.Application.Commons.Exceptions;

namespace Audiochan.Application.Features.Users.Exceptions;

public class UnmatchedPasswordException : BadRequestException
{
    public UnmatchedPasswordException() : base("Password does not match.")
    {
    }
}