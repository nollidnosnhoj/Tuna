using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Auth.CreateUser
{
    public class CreateUserCommand : IRequest<Result<bool>>
    {
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }

    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator(IOptions<IdentitySettings> identitySettings)
        {
            RuleFor(req => req.Username)
                .Username(identitySettings.Value.UsernameSettings);
            RuleFor(req => req.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is invalid.");
            RuleFor(req => req.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Password(identitySettings.Value.PasswordSettings);
        }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<bool>>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly UserManager<User> _userManager;

        public CreateUserCommandHandler(IDateTimeProvider dateTimeProvider, UserManager<User> userManager)
        {
            _dateTimeProvider = dateTimeProvider;
            _userManager = userManager;
        }

        public async Task<Result<bool>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var user = new User(command.Username.Trim().ToLower(), command.Email, _dateTimeProvider.Now);
            return (await _userManager.CreateAsync(user, command.Password)).ToResult();
        }
    }
}