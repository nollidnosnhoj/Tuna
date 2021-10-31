using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Auth
{
    public class CreateUserCommand : IRequest<Result>
    {
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUnitOfWork _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserCommandHandler(IDateTimeProvider dateTimeProvider, IUnitOfWork dbContext, IPasswordHasher passwordHasher)
        {
            _dateTimeProvider = dateTimeProvider;
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var trimmedUsername = command.Username.Trim();
            if (await _dbContext.Users.ExistsAsync(u => u.UserName == trimmedUsername, cancellationToken))
                return Result.BadRequest("Username already taken."); // Maybe a generic error message
            if (await _dbContext.Users.ExistsAsync(u => u.Email == command.Email, cancellationToken))
                return Result.BadRequest("Email already taken."); // Maybe a generic error message
            var passwordHash = _passwordHasher.Hash(command.Password);
            var user = new User(trimmedUsername, command.Email, passwordHash);
            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}