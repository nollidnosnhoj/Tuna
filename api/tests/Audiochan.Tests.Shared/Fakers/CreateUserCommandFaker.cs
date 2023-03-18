using Audiochan.Core.Features.Users.Commands;
using Bogus;

namespace Audiochan.Tests.Shared.Fakers;

public sealed class CreateUserCommandFaker : Faker<CreateUserCommand>
{
    public CreateUserCommandFaker()
    {
        CustomInstantiator(faker =>
            new CreateUserCommand(faker.Internet.UserName(), faker.Internet.Email(), faker.Internet.Password()));
    }
}