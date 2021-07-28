using Audiochan.Core.Interfaces;
using Moq;

namespace Audiochan.Tests.Common.Mocks
{
    public static class CurrentUserServiceMock
    {
        public const string MockUserId = "280e8105-20b1-455a-bfc5-e5ddc281eec8";
        public const string MockUserName = "testuser";
        
        public static Mock<ICurrentUserService> Create(string? userId = null, string? username = null)
        {
            var mock = new Mock<ICurrentUserService>();

            userId = string.IsNullOrWhiteSpace(userId) ? MockUserId : userId;
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