using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Audiochan.Core.Services;

public interface ITokenProvider
{
    string GenerateAccessToken(IEnumerable<Claim> claims, DateTime expiration);
}