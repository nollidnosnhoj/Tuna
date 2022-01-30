using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons;
using Audiochan.Application.Commons.Services;
using Audiochan.Application.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.Application.Commons.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Application.Features.Auth.Commands.Register
{
    public class CreateUserCommand : IRequest<Result>
    {
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result>
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

        public async Task<Result> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var trimmedUsername = command.Username.Trim();
            if (await _dbContext.Users.AnyAsync(u => u.UserName == trimmedUsername, cancellationToken))
                return new ErrorResult("Username already taken."); // Maybe a generic error message
            if (await _dbContext.Users.AnyAsync(u => u.Email == command.Email, cancellationToken))
                return new ErrorResult("Email already taken."); // Maybe a generic error message
            var passwordHash = _passwordHasher.Hash(command.Password);
            var user = new User(trimmedUsername, command.Email, passwordHash);
            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new SuccessResult();
        }
    }
}