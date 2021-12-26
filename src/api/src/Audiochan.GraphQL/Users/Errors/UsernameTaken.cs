using Audiochan.Application.Features.Users.Exceptions;
using Audiochan.GraphQL.Common.Errors;

namespace Audiochan.GraphQL.Users.Errors;

public class UsernameTaken : GraphQlError
{
    public string Username { get; }
    
    public UsernameTaken(UsernameTakenException ex) : base($"{ex.UserName} is already taken.")
    {
        Username = ex.UserName;
    }
}