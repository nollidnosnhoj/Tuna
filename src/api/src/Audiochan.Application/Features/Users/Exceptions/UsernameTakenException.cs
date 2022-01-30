using Audiochan.Application.Commons.Exceptions;

namespace Audiochan.Application.Features.Users.Exceptions;

public class UsernameTakenException : BadRequestException
{
    public UsernameTakenException(string username) : base($"{username} is already taken.")
    {
    }
}