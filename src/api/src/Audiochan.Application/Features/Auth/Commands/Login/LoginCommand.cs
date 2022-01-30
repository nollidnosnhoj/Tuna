using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Features.Auth.Queries.GetCurrentUser;
using Audiochan.Application.Persistence;
using AutoMapper;
using Audiochan.Application.Features.Auth.Exceptions;
using Audiochan.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Application.Features.Auth.Commands.Login
{
    public record LoginCommand : ICommandRequest<CurrentUserDto>
    {
        public string Login { get; init; } = null!;
        public string Password { get; init; } = null!;
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, CurrentUserDto>
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

        public async Task<CurrentUserDto> Handle(LoginCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Where(u => u.UserName == command.Login || u.Email == command.Login)
                .SingleOrDefaultAsync(cancellationToken);

            if (user == null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
                throw new LoginException();

            return _mapper.Map<CurrentUserDto>(user);
        }
    }
}