using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Auth.Revoke
{
    public record RevokeTokenRequest : IRequest<IResult<bool>>
    {
        public string RefreshToken { get; init; }
    }

    public class RevokeTokenRequestHandler : IRequestHandler<RevokeTokenRequest, IResult<bool>>
    {
        private readonly UserManager<User> _userManager;

        public RevokeTokenRequestHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IResult<bool>> Handle(RevokeTokenRequest request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                var user = await _userManager.Users
                    .Include(u => u.RefreshTokens)
                    .SingleOrDefaultAsync(u => u.RefreshTokens
                        .Any(r => r.Token == request.RefreshToken && u.Id == r.UserId), cancellationToken);

                if (user != null)
                {
                    var existingRefreshToken = user.RefreshTokens
                        .Single(r => r.Token == request.RefreshToken);

                    user.RefreshTokens.Remove(existingRefreshToken);

                    await _userManager.UpdateAsync(user);
                }
            }

            return Result<bool>.Success(true);
        }
    }
}