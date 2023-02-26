using System.Collections.Generic;
using System.Linq;
using Audiochan.Common.Models;
using Audiochan.Core.Features.Auth.Exceptions;
using Audiochan.Core.Features.Auth.Models;

namespace Audiochan.API.Features.Users.Errors;

public class UserIdentityError : IUserError
{
    public UserIdentityError(IEnumerable<IdentityError> errors)
    {
        Errors = errors.ToList();
    }
    
    public static UserIdentityError CreateErrorFrom(IdentityException exception)
    {
        return new UserIdentityError(exception.Result.Errors.Select(x => new IdentityError(x.Code, x.Message)));
    }

    public IReadOnlyCollection<IdentityError> Errors { get; }
    public string Code => GetType().Name;
    public string Message => "Identity error(s) has occurred.";
}