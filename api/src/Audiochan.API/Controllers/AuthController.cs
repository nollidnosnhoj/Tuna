using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Dtos;
using Audiochan.Core.Features.Auth.Dtos;
using Audiochan.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers
{
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IDateTimeProvider _dateTime;

        public AuthController(IDateTimeProvider dateTime, IAuthService authService)
        {
            _dateTime = dateTime;
            _authService = authService;
        }

        [HttpPost("login", Name = "Login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [SwaggerOperation(
            Summary = "Login using your credentials.",
            OperationId = "Login",
            Tags = new[] {"auth"}
        )]
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginWithPasswordAsync(request.Login, request.Password, cancellationToken);

            return Ok(result);
        }

        [HttpPost("logout")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Authorize]
        [SwaggerOperation(Summary = "Logout user", OperationId = "Logout", Tags = new[]{"auth"})]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
    }
}