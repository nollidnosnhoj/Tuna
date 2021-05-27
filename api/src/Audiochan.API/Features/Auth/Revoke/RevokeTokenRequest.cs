using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Auth.Refresh;
using Audiochan.Core.Entities;
using Audiochan.Core.Models;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;

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
                var user = await _unitOfWork.Users.GetAsync(
                        new GetUserBasedOnRefreshTokenSpecification(request.RefreshToken),
                        cancellationToken: cancellationToken);

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