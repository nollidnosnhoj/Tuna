using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Features.Auth.Models;

namespace Audiochan.Infrastructure.Identity.Extensions;

public static class IdentityExtensions
{
    public static IEnumerable<IdentityError> ToAppIdentityError(
        this IEnumerable<Microsoft.AspNetCore.Identity.IdentityError> errors)
    {
        return errors.Select(x => new IdentityError(x.Code, x.Description));
    }

    public static IdentityResult ToAppIdentityResult(this Microsoft.AspNetCore.Identity.IdentityResult result)
    {
        return new IdentityResult(result.Succeeded, result.Errors.ToAppIdentityError());
    }
}