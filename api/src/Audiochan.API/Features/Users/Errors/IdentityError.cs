using System.Collections.Generic;
using System.Linq;
using Audiochan.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.API.Features.Users.Errors;

public class IdentityErrors : IUserError
{
    public IReadOnlyList<IdentityError> Errors { get; }

    public IdentityErrors(IEnumerable<IdentityError> errors)
    {
        Errors = errors.ToList();
    }

    public string Code => "IdenityError";
    public string Message => "An identity error has occurred.";
}

