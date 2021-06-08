using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Auth.Login
{
    public record LoginCommand : IRequest<Result<LoginSuccessViewModel>>
    {
        public string Login { get; init; } = null!;
        public string Password { get; init; } = null!;
    }


    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginSuccessViewModel>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenProvider _tokenProvider;
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(UserManager<User> userManager, ITokenProvider tokenProvider, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _tokenProvider = tokenProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<LoginSuccessViewModel>> Handle(LoginCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .Where(u => u.UserName == command.Login || u.Email == command.Login)
                .SingleOrDefaultAsync(cancellationToken);

            if (user == null || !await _userManager.CheckPasswordAsync(user, command.Password))
                return Result<LoginSuccessViewModel>.Fail(ResultError.BadRequest, "Invalid Username/Password");

            var (accessToken, accessTokenExpiration) = await _tokenProvider.GenerateAccessToken(user);

            var (refreshToken, refreshTokenExpiration) = await _tokenProvider.GenerateRefreshToken(user);

            var result = new LoginSuccessViewModel
            {
                AccessToken = accessToken,
                AccessTokenExpires = accessTokenExpiration,
                RefreshToken = refreshToken,
                RefreshTokenExpires = refreshTokenExpiration
            };

            return Result<LoginSuccessViewModel>.Success(result);
        }
    }
}