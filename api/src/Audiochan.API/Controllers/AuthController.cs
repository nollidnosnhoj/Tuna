using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Services;
using Audiochan.Core.Features.Auth;
using Audiochan.Core.Features.Auth.Dtos;
using Audiochan.Core.Features.Auth.Validators;
using Audiochan.Core.Features.Users.Dtos;
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
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginRequest request,
            CancellationToken cancellationToken)
        {
            var validateResult = await new LoginRequestValidator().ValidateAsync(request, cancellationToken);

            if (!validateResult.IsValid)
            {
                return BadRequest(new { Message = "Login invalid.", Errors = validateResult.Errors });
            }
            
            var user = await _authService.LoginAsync(request, cancellationToken);

            if (user is null)
            {
                return BadRequest(new { Message = "Login invalid." });
            }

            Claim[] claims = {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties { ExpiresUtc = _dateTime.Now.AddDays(14) };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
            HttpContext.User = principal;

            return Ok(user);
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