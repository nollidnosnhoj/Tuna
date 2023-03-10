using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Audiochan.Core.Services;
using Microsoft.IdentityModel.Tokens;

namespace Audiochan.Infrastructure.Security;

public class JsonWebTokenService : ITokenService
{
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var secret = new SymmetricSecurityKey("audiochan_secretkey"u8.ToArray());   // TODO: add key to configuration
        var signingCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        var tokenOptions = new JwtSecurityToken(
            issuer: "http://localhost:5000",    // TODO: add to configuration
            audience: "http://localhost:5000",  // TODO: add to configuration
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: signingCredentials
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return tokenString;
    }
}