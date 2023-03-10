using System.Collections.Generic;
using System.Security.Claims;

namespace Audiochan.Core.Services;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
}