using Audiochan.Application.Features.Users.Exceptions;
using Audiochan.GraphQL.Common.Errors;

namespace Audiochan.GraphQL.Users.Errors;

public class UserNameTaken : GraphQlError
{
    public string UserName { get; }
    
    public UserNameTaken(UsernameTakenException ex) : base($"{ex.UserName} is already taken.")
    {
        UserName = ex.UserName;
    }
}