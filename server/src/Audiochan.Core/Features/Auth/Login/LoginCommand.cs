using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Auth.Login
{
    public record LoginCommand : IRequest<IResult<AuthResultViewModel>>
    {
        public string Login { get; init; }
        public string Password { get; init; }
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Login).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, IResult<AuthResultViewModel>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<IResult<AuthResultViewModel>> Handle(LoginCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .SingleOrDefaultAsync(u =>
                    u.UserName == request.Login.Trim().ToLower() || u.Email == request.Login, cancellationToken);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return Result<AuthResultViewModel>.Fail(ResultError.BadRequest, "Invalid Username/Password");

            var (token, tokenExpiration) = await _tokenService.GenerateAccessToken(user);

            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var result = new AuthResultViewModel
            {
                AccessToken = token,
                AccessTokenExpires = tokenExpiration,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpires = _tokenService.DateTimeToUnixEpoch(refreshToken.Expiry)
            };

            return Result<AuthResultViewModel>.Success(result);
        }
    }
}