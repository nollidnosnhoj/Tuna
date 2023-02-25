using System.Collections.Generic;
using System.Security.Claims;

namespace Audiochan.Core.Security;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
}