using Audiochan.Application.Commons.Exceptions;

namespace Audiochan.Application.Features.Users.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public long? UserId { get; }
    public string? UserName { get; }

    public UserNotFoundException(long userId) : base($"Unable to find user with id: {userId}")
    {
        UserId = userId;
    }
    
    public UserNotFoundException(string userName) : base($"Unable to find user with userName: {userName}")
    {
        UserName = userName;
    }
}