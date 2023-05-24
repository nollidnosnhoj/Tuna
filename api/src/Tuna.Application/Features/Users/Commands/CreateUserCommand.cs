using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using MediatR;
using Tuna.Application.Exceptions;
using Tuna.Application.Features.Users.Models;
using Tuna.Application.Persistence;
using Tuna.Application.Services;
using Tuna.Domain.Entities;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Users.Commands;

public class CreateUserCommand : ICommandRequest<Result<CurrentUserDto>>
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

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<CurrentUserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

    public async Task<Result<CurrentUserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var trimmedUsername = request.Username.Trim();

        var identityResult = await _userService.CreateUserAsync(
            trimmedUsername,
            request.Email,
            request.Password,
            cancellationToken);

        if (!identityResult.Succeeded)
            return new Result<CurrentUserDto>(new UserIdentityException(identityResult.Errors));

        var user = new User(identityResult.IdentityId, trimmedUsername);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CurrentUserDto
        {
            Id = user.Id,
            IdentityId = identityResult.IdentityId,
            Picture = user.ImageId,
            UserName = user.UserName
        };
    }
}