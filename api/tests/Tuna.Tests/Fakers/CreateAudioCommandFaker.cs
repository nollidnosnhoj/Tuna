using Bogus;
using Tuna.Application.Features.Users.Commands;

namespace Tuna.Tests.Fakers;

public sealed class CreateUserCommandFaker : Faker<CreateUserCommand>
{
    public CreateUserCommandFaker()
    {
        CustomInstantiator(faker =>
            new CreateUserCommand(faker.Internet.UserName(), faker.Internet.Email(), faker.Internet.Password()));
    }
}