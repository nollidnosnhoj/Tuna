using Audiochan.Core.Interfaces;
using Moq;

namespace Audiochan.UnitTests.Mocks
{
    public static class CurrentUserServiceMock
    {
        public static Mock<ICurrentUserService> Create(string userId = "00000000-0000-0000-0000-000000000000",
            string username = "")
        {
            var mock = new Mock<ICurrentUserService>();
            mock.Setup(x => x.GetUserId()).Returns(userId);
            mock.Setup(x => x.GetUsername()).Returns(username);
            mock.Setup(x => x.IsAuthenticated()).Returns(true);
            return mock;
        }
    }
}