using Audiochan.Domain.Entities;
using Bogus;

namespace Audiochan.Tests.Common.Fakers.Users;

public sealed class UserFaker : Faker<User>
{
    public UserFaker()
    {
        CustomInstantiator(faker => new User(faker.Internet.UserName(), faker.Internet.ExampleEmail(), faker.Internet.Password()));
    }
}