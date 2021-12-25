using Audiochan.Application.Commons.Exceptions;

namespace Audiochan.Application.Features.Auth.Exceptions;

public class LoginException : BadRequestException
{
    public LoginException() : base("Invalid username/password.")
    {
    }
}