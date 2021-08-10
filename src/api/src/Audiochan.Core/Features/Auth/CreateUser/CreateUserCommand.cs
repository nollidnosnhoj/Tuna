using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Auth.CreateUser
{
    public class CreateUserCommand : IRequest<Result>
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

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ApplicationDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserCommandHandler(IDateTimeProvider dateTimeProvider, ApplicationDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _dateTimeProvider = dateTimeProvider;
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var trimmedUsername = command.Username.Trim();
            if (await _dbContext.Users.AnyAsync(u => u.UserName == trimmedUsername, cancellationToken))
                return Result.BadRequest("Username already taken."); // Maybe a generic error message
            if (await _dbContext.Users.AnyAsync(u => u.Email == command.Email, cancellationToken))
                return Result.BadRequest("Email already taken."); // Maybe a generic error message
            var passwordHash = _passwordHasher.Hash(command.Password);
            var user = new User(trimmedUsername, command.Email, passwordHash);
            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}