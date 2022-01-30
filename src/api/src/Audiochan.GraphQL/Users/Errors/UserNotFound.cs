using Audiochan.Application.Commons.Exceptions;
using Audiochan.Domain.Entities;
using Audiochan.GraphQL.Common.Errors;

namespace Audiochan.GraphQL.Users.Errors;

public class UserNotFound : GraphQlError
{
    public long UserId { get; }
    public UserNotFound(NotFoundException<User, long> exception) 
        : base($"Resource is not found. Type: {exception.Type.Name}. Id: {exception.ResourceId}")
    {
        UserId = exception.ResourceId;
    }
}