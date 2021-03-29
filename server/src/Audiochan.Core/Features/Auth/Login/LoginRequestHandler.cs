using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Auth.Login
{
    public class LoginRequestHandler : IRequestHandler<LoginRequest, IResult<AuthResultViewModel>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenProvider _tokenProvider;

        public LoginRequestHandler(UserManager<User> userManager, ITokenProvider tokenProvider)
        {
            _userManager = userManager;
            _tokenProvider = tokenProvider;
        }

        public async Task<IResult<AuthResultViewModel>> Handle(LoginRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .SingleOrDefaultAsync(u =>
                    u.UserName == request.Login.Trim().ToLower() || u.Email == request.Login, cancellationToken);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return Result<AuthResultViewModel>.Fail(ResultError.BadRequest, "Invalid Username/Password");

            var (token, tokenExpiration) = await _tokenProvider.GenerateAccessToken(user);

            var refreshToken = _tokenProvider.GenerateRefreshToken(user.Id);

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var result = new AuthResultViewModel
            {
                AccessToken = token,
                AccessTokenExpires = tokenExpiration,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpires = _tokenProvider.DateTimeToUnixEpoch(refreshToken.Expiry)
            };

            return Result<AuthResultViewModel>.Success(result);
        }
    }
}