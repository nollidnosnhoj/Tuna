using Audiochan.Common.Models;
using Audiochan.Core.Features.Auth.Exceptions;

namespace Audiochan.API.Features.Users.Errors;

public class IdentityError : IUserError
{
    public IdentityError(IdentityException exception)
    {
        Code = exception.Error.Code;
        Message = exception.Error.Description;
    }

    public string Code { get; }
    public string Message { get; }
}

