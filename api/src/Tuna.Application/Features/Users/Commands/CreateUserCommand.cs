﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OneOf;
using Tuna.Application.Features.Users.Errors;
using Tuna.Application.Features.Users.Models;
using Tuna.Application.Persistence;
using Tuna.Application.Services;
using Tuna.Domain.Entities;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Users.Commands;

public class CreateUserCommand : ICommandRequest<CreateUserCommandResult>
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

[GenerateOneOf]
public partial class CreateUserCommandResult : OneOfBase<CurrentUserDto, IdentityServiceError>
{
}

// public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
// {
//     public CreateUserCommandValidator()
//     {
//     }
// }

public class CreateUserCommandResponse
{
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResult>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

    public async Task<CreateUserCommandResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var trimmedUsername = request.Username.Trim();

        var identityResult = await _userService.CreateUserAsync(
            trimmedUsername,
            request.Email,
            request.Password,
            cancellationToken);

        if (!identityResult.Succeeded) return new IdentityServiceError(identityResult.Errors);

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