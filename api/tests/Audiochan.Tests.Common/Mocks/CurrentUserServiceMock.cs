using System.Collections.Generic;
using System.Security.Claims;
using Audiochan.Core.Services;
using Moq;

namespace Audiochan.Tests.Common.Mocks
{
    public static class CurrentUserServiceMock
    {
        public const long MockUserId = 1996;
        public const string MockUserName = "testuser";
        
        public static Mock<ICurrentUserService> Create(ClaimsPrincipal? principal = null)
        {
            var mock = new Mock<ICurrentUserService>();
            mock.Setup(x => x.User).Returns(principal);
            return mock;
        }

        public static ClaimsPrincipal CreateMockPrincipal(long userId, string username)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, username)
            };

            return new ClaimsPrincipal(new ClaimsIdentity(claims));
        }
    }
}