using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Tuna.Application.Services;

public interface ITokenProvider
{
    string GenerateAccessToken(IEnumerable<Claim> claims, DateTime expiration);
    string GenerateRefreshToken();
}