using Audiochan.Application.Commons.Exceptions;

namespace Audiochan.Application.Features.Users.Exceptions;

public class UsernameTakenException : BadRequestException
{
    public string UserName { get; }
    
    public UsernameTakenException(string username) : base("Username is already taken.")
    {
        UserName = username;
    }
}