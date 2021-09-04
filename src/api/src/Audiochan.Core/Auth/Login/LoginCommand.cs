using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using Audiochan.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Audiochan.Core.Auth.Login
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

    public sealed class LoadUserForLoginSpecification : Specification<User>
    {
        public LoadUserForLoginSpecification(string login)
        {
            Query.Include(u => u.RefreshTokens);
            Query.Where(u => u.UserName == login || u.Email == login);
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResultViewModel>>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IUnitOfWork _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public LoginCommandHandler(ITokenProvider tokenProvider, IUnitOfWork dbContext, IPasswordHasher passwordHasher)
        {
            _tokenProvider = tokenProvider;
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<AuthResultViewModel>> Handle(LoginCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .GetFirstAsync(new LoadUserForLoginSpecification(command.Login), cancellationToken);

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