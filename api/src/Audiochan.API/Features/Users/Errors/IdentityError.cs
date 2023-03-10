using Audiochan.Core.Exceptions;
using Audiochan.Shared.Models;

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

