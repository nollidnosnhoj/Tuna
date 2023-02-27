using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Features.Auth.Models;

namespace Audiochan.Core.Features.Users.Errors;

public struct UserIdentityError
{
    public IReadOnlyList<IdentityError> Errors { get; }

    public UserIdentityError(IEnumerable<IdentityError> errors)
    {
        Errors = errors.ToList();
    }
}