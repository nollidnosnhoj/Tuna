using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.API.Features.Auth.Revoke
{
    public record RevokeTokenRequest : IRequest<Result<bool>>
    {
        public string RefreshToken { get; init; } = null!;
    }

    public class RevokeTokenRequestHandler : IRequestHandler<RevokeTokenRequest, Result<bool>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public RevokeTokenRequestHandler(UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(RevokeTokenRequest request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                var user = await _unitOfWork.Users
                    .Include(u => u.RefreshTokens)
                    .SingleOrDefaultAsync(u => u.RefreshTokens
                        .Any(r => r.Token == request.RefreshToken && u.Id == r.UserId), cancellationToken);

                if (user != null)
                {
                    var existingRefreshToken = user.RefreshTokens
                        .Single(r => r.Token == request.RefreshToken);

                    user.RefreshTokens.Remove(existingRefreshToken);

                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }

            return Result<bool>.Success(true);
        }
    }
}