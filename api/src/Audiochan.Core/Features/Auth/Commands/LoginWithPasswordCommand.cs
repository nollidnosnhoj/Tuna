using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Auth.Commands;

public class LoginWithPasswordCommand : ICommandRequest<IdentityResult>
{
    public LoginWithPasswordCommand(string login, string password)
    {
        Login = login;
        Password = password;
    }
    
    public string Login { get; }
    public string Password { get; }
}

public class LoginWithPasswordCommandHandler : IRequestHandler<LoginWithPasswordCommand, IdentityResult>
{
    private readonly UserManager<User> _userManager;

    public LoginWithPasswordCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> Handle(LoginWithPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Login);

        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "User Invalid",
                Description = "User was not found"
            });
        }

        var valid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!valid)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "Password failed",
                Description = "Password was incorrect."
            });
        }

        return IdentityResult.Success;
    }
}