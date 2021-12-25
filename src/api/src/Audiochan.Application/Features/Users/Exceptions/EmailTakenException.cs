using Audiochan.Application.Commons.Exceptions;

namespace Audiochan.Application.Features.Users.Exceptions;

public class EmailTakenException : BadRequestException
{
    public EmailTakenException(string email) : base($"{email} is already taken.")
    {
    }
}