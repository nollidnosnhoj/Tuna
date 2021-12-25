using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Services;
using Audiochan.Application.Features.Auth.Exceptions;
using Audiochan.Application.Features.Users.Exceptions;
using Audiochan.Application.Persistence;
using Audiochan.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Application.Features.Auth.Commands.Register
{
    public class CreateUserCommand : ICommandRequest
    {
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, 
            ApplicationDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var trimmedUsername = command.Username.Trim();
            if (await _dbContext.Users.AnyAsync(u => u.UserName == trimmedUsername, cancellationToken))
                throw new UsernameTakenException(command.Username);
            if (await _dbContext.Users.AnyAsync(u => u.Email == command.Email, cancellationToken))
                throw new EmailTakenException(command.Email);
            var passwordHash = _passwordHasher.Hash(command.Password);
            var user = new User(trimmedUsername, command.Email, passwordHash);
            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}