using Tuna.Application.Exceptions;
using Tuna.Shared.Models;

namespace Tuna.GraphQl.Features.Users.Errors;

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

