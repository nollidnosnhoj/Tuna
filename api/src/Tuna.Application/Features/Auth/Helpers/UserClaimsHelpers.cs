using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Tuna.Application.Entities;
using Tuna.Shared;

namespace Tuna.Application.Features.Auth.Helpers;

public static class UserClaimsHelpers
{
    public static IEnumerable<Claim> ToClaims(string identityId, User? user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, identityId),
            new(ClaimNames.HasProfile, (user is not null).ToString())
        };

        if (user is not null)
        {
            claims.Add(new Claim(ClaimNames.UserId, user.Id.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, user.UserName));
        }

        return claims;
    }
}