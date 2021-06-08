using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Auth.Revoke
{
    public record RevokeTokenCommand : IRequest<Result<bool>>
    {
        public string RefreshToken { get; init; } = null!;
    }

    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, Result<bool>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public RevokeTokenCommandHandler(UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(RevokeTokenCommand command, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(command.RefreshToken))
            {
                var user = await _unitOfWork.Users
                    .Include(u => u.RefreshTokens)
                    .SingleOrDefaultAsync(u => u.RefreshTokens
                        .Any(r => r.Token == command.RefreshToken && u.Id == r.UserId), cancellationToken);

                if (user != null)
                {
                    var existingRefreshToken = user.RefreshTokens
                        .Single(r => r.Token == command.RefreshToken);

                    user.RefreshTokens.Remove(existingRefreshToken);

                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }

            return Result<bool>.Success(true);
        }
    }
}