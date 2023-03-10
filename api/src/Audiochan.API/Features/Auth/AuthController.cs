using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Auth.Models;
using Audiochan.Core.Features.Auth;
using Audiochan.Core.Features.Auth.Models;
using Audiochan.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Features.Auth;

[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login-with-password", Name = "Login")]
    [ProducesResponseType(200)]
    [ProducesResponseType(402)]
    [SwaggerOperation(
        Summary = "Login using your credentials.",
        OperationId = "Login",
        Tags = new[] {"auth"}
    )]
    public async Task<IActionResult> LoginWithPassword([FromBody] LoginWithPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginWithPasswordAsync(request.Login, request.Password, cancellationToken);

        return result.Match<IActionResult>(
            authTokenResult => new OkObjectResult(authTokenResult),
            _ => new UnauthorizedResult());
    }

    [HttpPost("logout")]
    [ProducesResponseType(200)]
    [Authorize]
    [SwaggerOperation(Summary = "Logout user", OperationId = "Logout", Tags = new[]{"auth"})]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken cancellationToken)
    {
        var refreshToken = !string.IsNullOrEmpty(request.RefreshToken)
            ? request.RefreshToken
            : Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return BadRequest(new UserError("InvalidRefreshToken", "Refresh token is invalid"));
        }
        
        await _authService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);
        return Ok();
    }
}