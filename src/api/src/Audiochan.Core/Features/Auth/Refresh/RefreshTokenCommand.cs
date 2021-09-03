using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Audiochan.Core.Features.Auth.Refresh
{
    public record RefreshTokenCommand : IRequest<Result<AuthResultViewModel>>
    {
        public string RefreshToken { get; init; } = null!;
    }
    
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token is required.");
        }
    }

    public sealed class LoadUserByRefreshToken : Specification<User>
    {
        public LoadUserByRefreshToken(string refreshToken)
        {
            Query.Include(u => u.RefreshTokens);
            Query.Where(u => u.RefreshTokens.Any(t => t.Token == refreshToken && t.UserId == u.Id));
        }
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResultViewModel>>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IUnitOfWork _dbContext;

        public RefreshTokenCommandHandler(ITokenProvider tokenProvider, IUnitOfWork dbContext)
        {
            _tokenProvider = tokenProvider;
            _dbContext = dbContext;
        }

        public async Task<Result<AuthResultViewModel>> Handle(RefreshTokenCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .GetFirstAsync(new LoadUserByRefreshToken(command.RefreshToken), cancellationToken);

            if (user == null)
                return Result<AuthResultViewModel>.BadRequest("Refresh token does not belong to a user.");

            var existingRefreshToken = user.RefreshTokens
                .Single(r => r.Token == command.RefreshToken);

            if (!await _tokenProvider.ValidateRefreshToken(existingRefreshToken.Token))
                return Result<AuthResultViewModel>.BadRequest("Refresh token is invalid/expired.");

            var (refreshToken, refreshTokenExpiration) =
                await _tokenProvider.GenerateRefreshToken(user, existingRefreshToken.Token);
            var (token, tokenExpiration) = _tokenProvider.GenerateAccessToken(user);

            return Result<AuthResultViewModel>.Success(new AuthResultViewModel
            {
                AccessToken = token,
                AccessTokenExpires = tokenExpiration,
                RefreshToken = refreshToken,
                RefreshTokenExpires = refreshTokenExpiration
            });
        }
    }
}