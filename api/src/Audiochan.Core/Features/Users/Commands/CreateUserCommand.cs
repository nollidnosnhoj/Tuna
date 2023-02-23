using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Extensions;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using Audiochan.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Audiochan.Core.Features.Users.Commands;

public class CreateUserCommand : ICommandRequest<long>
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

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(req => req.Username)
            .UsernameValidation();
        RuleFor(req => req.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is invalid.");
        RuleFor(req => req.Password)
            .NotEmpty().WithMessage("Password is required.")
            .PasswordValidation();
    }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task<long> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var trimmedUsername = request.Username.Trim();

        var identityResult = await _identityService.CreateUserAsync(
            trimmedUsername,
            request.Email,
            request.Password,
            cancellationToken);
        
        identityResult.EnsureSuccessfulResult();

        var user = new User(identityResult.IdentityId, trimmedUsername);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return user.Id;
    }
}