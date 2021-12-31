using Audiochan.Application.Features.Auth.Exceptions;
using Audiochan.GraphQL.Common.Errors;

namespace Audiochan.GraphQL.Auth.Errors;

public class SignInError : GraphQlError
{
    public SignInError(LoginException exception) : base(exception.Message)
    {
    }
}