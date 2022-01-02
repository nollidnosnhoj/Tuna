using Audiochan.Application.Commons.Exceptions;
using Audiochan.Application.Features.Users.Exceptions;
using Audiochan.Domain.Entities;
using Audiochan.GraphQL.Common.Errors;

namespace Audiochan.GraphQL.Users.Errors;

public class UserNotFound : GraphQlError
{
    public long? UserId { get; }
    public string? UserName { get; }
    public UserNotFound(UserNotFoundException exception) 
        : base(exception.Message)
    {
        UserId = exception.UserId;
        UserName = exception.UserName;
    }
}