using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Auth.Queries;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using AutoMapper;
using MediatR;

namespace Audiochan.Core.Auth.Commands
{
    public record LoginCommand : IRequest<Result<CurrentUserDto>>
    {
        public string Login { get; init; } = null!;
        public string Password { get; init; } = null!;
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<CurrentUserDto>>
    {
        private readonly IUnitOfWork _dbContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public LoginCommandHandler(IUnitOfWork dbContext, IPasswordHasher passwordHasher, IMapper mapper)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<Result<CurrentUserDto>> Handle(LoginCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .GetFirstAsync(new LoadUserByNameOrEmailSpecification(command.Login), cancellationToken);

            if (user == null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
                return Result<CurrentUserDto>.BadRequest("Invalid Username/Password");

            var currentUser = _mapper.Map<CurrentUserDto>(user);

            return Result<CurrentUserDto>.Success(currentUser);
        }
    }
}