using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Tuna.Application.Features.Users.Errors;

public struct IdentityServiceError
{
    public IReadOnlyList<IdentityError> Errors { get; }

    public IdentityServiceError(IEnumerable<IdentityError> errors)
    {
        Errors = errors.ToList();
    }
}