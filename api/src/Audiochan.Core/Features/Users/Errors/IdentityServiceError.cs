using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Features.Auth.Models;

namespace Audiochan.Core.Features.Users.Errors;

public struct IdentityServiceError
{
    public IReadOnlyList<IdentityError> Errors { get; }

    public IdentityServiceError(IEnumerable<IdentityError> errors)
    {
        Errors = errors.ToList();
    }
}