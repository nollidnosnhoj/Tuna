using Audiochan.Common.Models;
using Audiochan.Core.Features.Users.Exceptions;

namespace Audiochan.API.Features.Users.Errors;

public class FollowError : IUserError
{
    public FollowError(string message)
    {
        Message = message;
    }

    public static FollowError CreateErrorFrom(CannotFollowYourselfException exception)
    {
        return new FollowError(exception.Message);
    }
    
    public string Code => GetType().Name;
    public string Message { get; }
}