using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Auth.Revoke
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
        private readonly ApplicationDbContext _dbContext;

        public RevokeTokenCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<bool>> Handle(RevokeTokenCommand command, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(command.RefreshToken))
            {
                var user = await _dbContext.Users
                    .Include(u => u.RefreshTokens)
                    .SingleOrDefaultAsync(u => u.RefreshTokens
                        .Any(t => t.Token == command.RefreshToken && t.UserId == u.Id), cancellationToken);

                if (user != null)
                {
                    var existingRefreshToken = user.RefreshTokens
                        .Single(r => r.Token == command.RefreshToken);

                    user.RefreshTokens.Remove(existingRefreshToken);

                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }

            return Result<bool>.Success(true);
        }
    }
}