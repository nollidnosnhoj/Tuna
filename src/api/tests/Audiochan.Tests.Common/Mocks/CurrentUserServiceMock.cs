using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Interfaces;
using Moq;

namespace Audiochan.Tests.Common.Mocks
{
    public static class CurrentUserServiceMock
    {
        public const long MockUserId = 1996;
        public const string MockUserName = "testuser";
        
        public static Mock<ICurrentUserService> Create(long userId = 0, string? username = null)
        {
            var mock = new Mock<ICurrentUserService>();

            userId = !UserHelpers.IsValidId(userId) ? MockUserId : userId;
            username = string.IsNullOrWhiteSpace(username) ? MockUserName : username;
            
            mock.Setup(x => x.GetUserId()).Returns(userId);
            mock.Setup(x => x.GetUsername()).Returns(username);
            mock.Setup(x => x.IsAuthenticated()).Returns(true);
            mock.Setup(x => x.TryGetUserId(out userId))
                .Returns(true);
            mock.Setup(x => x.TryGetUsername(out username))
                .Returns(true);
            return mock;
        }
    }
}