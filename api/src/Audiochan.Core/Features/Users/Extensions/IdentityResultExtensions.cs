using Audiochan.Core.Features.Auth.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Users.Extensions;

public static class IdentityResultExtensions
{
    public static void EnsureSuccessful(this IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new IdentityException(result);
        }
    }
}