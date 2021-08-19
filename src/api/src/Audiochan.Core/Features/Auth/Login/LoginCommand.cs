using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Auth.Login
{
    public record LoginCommand : IRequest<Result<AuthResultViewModel>>
    {
        public string Login { get; init; } = null!;
        public string Password { get; init; } = null!;
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Login).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResultViewModel>>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly ApplicationDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public LoginCommandHandler(ITokenProvider tokenProvider, ApplicationDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _tokenProvider = tokenProvider;
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<AuthResultViewModel>> Handle(LoginCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Include(u => u.RefreshTokens)
                .Where(u => u.UserName == command.Login || u.Email == command.Login)
                .SingleOrDefaultAsync(cancellationToken);

            if (user == null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
                return Result<AuthResultViewModel>.BadRequest("Invalid Username/Password");


            var (accessToken, accessTokenExpiration) = _tokenProvider.GenerateAccessToken(user);

            var (refreshToken, refreshTokenExpiration) = await _tokenProvider.GenerateRefreshToken(user);

            var result = new AuthResultViewModel
            {
                AccessToken = accessToken,
                AccessTokenExpires = accessTokenExpiration,
                RefreshToken = refreshToken,
                RefreshTokenExpires = refreshTokenExpiration
            };

            return Result<AuthResultViewModel>.Success(result);
        }
    }
}