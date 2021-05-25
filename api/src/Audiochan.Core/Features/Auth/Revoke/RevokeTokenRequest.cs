using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Auth.Refresh;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Auth.Revoke
{
    public record RevokeTokenRequest : IRequest<IResult<bool>>
    {
        public string RefreshToken { get; init; } = null!;
    }

    public class RevokeTokenRequestHandler : IRequestHandler<RevokeTokenRequest, IResult<bool>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public RevokeTokenRequestHandler(UserManager<User> userManager, IUserRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public async Task<IResult<bool>> Handle(RevokeTokenRequest request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                var user = await _userRepository.GetBySpecAsync(
                    new GetUserBasedOnRefreshTokenSpecification(request.RefreshToken),
                    cancellationToken: cancellationToken);

                if (user != null)
                {
                    var existingRefreshToken = user.RefreshTokens
                        .Single(r => r.Token == request.RefreshToken);

                    user.RefreshTokens.Remove(existingRefreshToken);

                    await _userRepository.UpdateAsync(user, cancellationToken);
                }
            }

            return Result<bool>.Success(true);
        }
    }
}