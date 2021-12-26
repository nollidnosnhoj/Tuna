using Audiochan.Application.Features.Users.Exceptions;
using Audiochan.GraphQL.Common.Errors;

namespace Audiochan.GraphQL.Users.Errors;

public class UnmatchedPassword : GraphQlError
{
    public UnmatchedPassword(UnmatchedPasswordException ex) : base(ex.Message)
    {
        
    }
}