using Audiochan.Application.Commons.Exceptions;

namespace Audiochan.Application.Features.Users.Exceptions;

public class UnmatchPasswordException : BadRequestException
{
    public UnmatchPasswordException() : base("Password does not match.")
    {
    }
}