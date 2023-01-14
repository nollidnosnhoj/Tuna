using System.Collections.Generic;
using System.Security.Claims;
using Audiochan.Core.Services;
using Moq;

namespace Audiochan.Tests.Common.Mocks;

public class MockCurrentService : ICurrentUserService
{
    public MockCurrentService(long userId, string userName)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, userName)
        };

        User = new ClaimsPrincipal(new ClaimsIdentity(claims));
    }

    public MockCurrentService(ClaimsPrincipal? user)
    {
        User = user;
    }
        
    public ClaimsPrincipal? User { get; }
}