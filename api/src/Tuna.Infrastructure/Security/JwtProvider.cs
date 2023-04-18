using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tuna.Application.Services;

namespace Tuna.Infrastructure.Security;

public class JwtProvider : ITokenProvider
{
    private readonly JwtOptions _options;

    public JwtProvider(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims, DateTime expiration)
    {
        var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var signingCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        var tokenOptions = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            expires: expiration,
            signingCredentials: signingCredentials
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return tokenString;
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToString(RandomNumberGenerator.GetBytes(16)) + "";
    }
}