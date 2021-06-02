using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.API.Features.Auth.Login
{
    public record LoginRequest : IRequest<Result<LoginSuccessViewModel>>
    {
        public string Login { get; init; } = null!;
        public string Password { get; init; } = null!;
    }


    public class LoginRequestHandler : IRequestHandler<LoginRequest, Result<LoginSuccessViewModel>>
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

        public async Task<Result<LoginSuccessViewModel>> Handle(LoginRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users
                .SingleOrDefaultAsync(u => u.UserName == request.Login.Trim().ToLower() 
                                           || u.Email == request.Login, cancellationToken);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
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