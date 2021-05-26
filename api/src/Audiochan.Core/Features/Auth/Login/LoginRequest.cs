using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Auth.Login
{
    public record LoginRequest : IRequest<IResult<AuthResultViewModel>>
    {
        public string Login { get; init; } = null!;
        public string Password { get; init; } = null!;
    }


    public class LoginRequestHandler : IRequestHandler<LoginRequest, IResult<AuthResultViewModel>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenProvider _tokenProvider;
        private readonly IUnitOfWork _unitOfWork;

        public LoginRequestHandler(UserManager<User> userManager, ITokenProvider tokenProvider, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _tokenProvider = tokenProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult<AuthResultViewModel>> Handle(LoginRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetBySpecAsync(new GetUserBasedOnLoginSpecification(request.Login),
                cancellationToken: cancellationToken);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return Result<AuthResultViewModel>.Fail(ResultError.BadRequest, "Invalid Username/Password");

            var (accessToken, accessTokenExpiration) = await _tokenProvider.GenerateAccessToken(user);

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