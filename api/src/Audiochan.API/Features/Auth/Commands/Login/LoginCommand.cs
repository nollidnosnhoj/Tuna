using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Auth.Queries;
using Audiochan.Core.CQRS;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Auth.Commands
{
    public record LoginCommand : ICommandRequest<Result<CurrentUserDto>>
    {
        public string Login { get; init; } = null!;
        public string Password { get; init; } = null!;
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<CurrentUserDto>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public LoginCommandHandler(ApplicationDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<CurrentUserDto>> Handle(LoginCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Where(u => u.UserName == command.Login || u.Email == command.Login)
                .SingleOrDefaultAsync(cancellationToken);

            if (user == null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
                return Result<CurrentUserDto>.BadRequest("Invalid Username/Password");

            var currentUser = new CurrentUserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };

            return Result<CurrentUserDto>.Success(currentUser);
        }
    }
}