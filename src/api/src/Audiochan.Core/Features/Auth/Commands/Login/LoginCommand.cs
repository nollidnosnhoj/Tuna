using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Commons;
using Audiochan.Core.Commons.CQRS;

using Audiochan.Core.Commons.Services;
using Audiochan.Core.Features.Auth.Queries.GetCurrentUser;
using Audiochan.Core.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Auth.Commands.Login
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
        private readonly IMapper _mapper;

        public LoginCommandHandler(ApplicationDbContext dbContext, IPasswordHasher passwordHasher, IMapper mapper)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<Result<CurrentUserDto>> Handle(LoginCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Where(u => u.UserName == command.Login || u.Email == command.Login)
                .SingleOrDefaultAsync(cancellationToken);

            if (user == null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
                return Result<CurrentUserDto>.BadRequest("Invalid Username/Password");

            var currentUser = _mapper.Map<CurrentUserDto>(user);

            return Result<CurrentUserDto>.Success(currentUser);
        }
    }
}