using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Features.Auth;
using Audiochan.Core.Features.Users.Errors;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Core.Persistence;
using Audiochan.Domain.Entities;
using MediatR;
using OneOf;

namespace Audiochan.Core.Features.Users.Commands;

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
public partial class CreateUserCommandResult : OneOfBase<UserDto, IdentityServiceError>
{
    
}

// public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
// {
//     public CreateUserCommandValidator()
//     {
//     }
// }

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task<CreateUserCommandResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var trimmedUsername = request.Username.Trim();

        var identityResult = await _identityService.CreateUserAsync(
            trimmedUsername,
            request.Email,
            request.Password,
            cancellationToken);

        if (!identityResult.Succeeded)
        {
            return new IdentityServiceError(identityResult.Errors);
        }

        var user = new User(identityResult.IdentityId, trimmedUsername);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Picture = user.ImageId,
            UserName = user.UserName
        };
    }
}