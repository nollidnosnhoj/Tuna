using Audiochan.Core.Features.Auth.Login;
using Bogus;

namespace Audiochan.Tests.Common.Fakers.Auth
{
    public sealed class LoginRequestFaker : Faker<LoginCommand>
    {
        public LoginRequestFaker()
        {
            RuleFor(x => x.Login, f => f.Random.String2(15));
            RuleFor(x => x.Password, f => f.Internet.Password());
        }
    }
}