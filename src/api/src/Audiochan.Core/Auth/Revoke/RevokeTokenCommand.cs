using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Auth.Refresh;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Models;
using FluentValidation;
using MediatR;

namespace Audiochan.Core.Auth.Revoke
{
    public record RevokeTokenCommand : IRequest<Result<bool>>
    {
        public string RefreshToken { get; init; } = null!;
    }
    
    public class RevokeTokenCommandValidator : AbstractValidator<RevokeTokenCommand>
    {
        public RevokeTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token is required.");
        }
    }

    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, Result<bool>>
    {
        private readonly IUnitOfWork _dbContext;

        public RevokeTokenCommandHandler(IUnitOfWork dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<bool>> Handle(RevokeTokenCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(command.RefreshToken)) 
                return Result<bool>.Success(true);
            
            var user = await _dbContext.Users
                .GetFirstAsync(new LoadUserByRefreshToken(command.RefreshToken), cancellationToken);

            if (user == null) 
                return Result<bool>.Success(true);
                
            var existingRefreshToken = user.RefreshTokens
                .Single(r => r.Token == command.RefreshToken);

            user.RefreshTokens.Remove(existingRefreshToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}