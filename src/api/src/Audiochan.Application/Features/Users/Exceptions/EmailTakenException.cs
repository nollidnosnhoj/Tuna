using Audiochan.Application.Commons.Exceptions;

namespace Audiochan.Application.Features.Users.Exceptions;

public class EmailTakenException : BadRequestException
{
    public string Email { get; }
    
    public EmailTakenException(string email) : base($"Email is already taken.")
    {
        Email = email;
    }
}