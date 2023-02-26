using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Auth;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Users.Commands;

public class CreateUserCommand : ICommandRequest<IdentityResult>
{
    public CreateUserCommand(string username, string email, string password)
    {
        Username = username;
        Email = email;
        Password = password;
    }

    public string Username { get; }
    public string Email { get; }
    public string Password { get; }
}

// public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
// {
//     public CreateUserCommandValidator()
//     {
//     }
// }

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IdentityResult>
{
    private readonly UserManager<User> _userManager;

    public CreateUserCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var trimmedUsername = request.Username.Trim();
        var user = new User(trimmedUsername, request.Email);
        return await _userManager.CreateAsync(user, request.Password);
    }
}