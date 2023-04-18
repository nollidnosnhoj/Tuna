using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tuna.Application.Features.Auth;
using Tuna.Application.Features.Auth.Models;
using Tuna.Application.Services;
using Tuna.GraphQl.Features.Auth.Extensions;
using Tuna.GraphQl.Features.Auth.Models;
using Tuna.Shared.Models;

namespace Tuna.GraphQl.Features.Auth;

[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuthController(IAuthService authService, IDateTimeProvider dateTimeProvider)
    {
        _authService = authService;
        _dateTimeProvider = dateTimeProvider;
    }

    [HttpPost("login-with-password", Name = "Login")]
    [ProducesResponseType(typeof(AuthServiceTokens), 200)]
    [ProducesResponseType(402)]
    [SwaggerOperation(
        Summary = "Login using your credentials.",
        OperationId = "Login",
        Tags = new[] { "auth" }
    )]
    public async Task<IActionResult> LoginWithPassword([FromBody] LoginWithPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _authService.LoginWithPasswordAsync(request.Login, request.Password, IpAddress(), cancellationToken);

        return result.ToActionResult(
            authTokenResult =>
            {
                SetRefreshTokenCookie(authTokenResult.RefreshToken);
                return new OkObjectResult(authTokenResult);
            },
            _ => new UnauthorizedResult());
    }

    [HttpPost("refreshToken", Name = "RefreshToken")]
    [ProducesResponseType(typeof(AuthServiceTokens), 200)]
    [ProducesResponseType(402)]
    [SwaggerOperation(
        Summary = "Refresh Access token.",
        OperationId = "RefreshToken",
        Tags = new[] { "auth" }
    )]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var refreshToken = !string.IsNullOrEmpty(request.RefreshToken)
            ? request.RefreshToken
            : Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest(new UserError("InvalidRefreshToken", "Refresh token is invalid"));

        var result = await _authService.RefreshTokenAsync(refreshToken, IpAddress(), cancellationToken);

        return result.ToActionResult(
            authTokenResult =>
            {
                SetRefreshTokenCookie(authTokenResult.RefreshToken);
                return new OkObjectResult(authTokenResult);
            },
            _ => new UnauthorizedResult());
    }

    [HttpPost("logout")]
    [ProducesResponseType(200)]
    [Authorize]
    [SwaggerOperation(Summary = "Logout user", OperationId = "Logout", Tags = new[] { "auth" })]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken cancellationToken)
    {
        var refreshToken = !string.IsNullOrEmpty(request.RefreshToken)
            ? request.RefreshToken
            : Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest(new UserError("InvalidRefreshToken", "Refresh token is invalid"));

        await _authService.RevokeRefreshTokenAsync(refreshToken, IpAddress(), cancellationToken);
        return Ok();
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Secure = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

    private string? IpAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"];
        return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
    }
}