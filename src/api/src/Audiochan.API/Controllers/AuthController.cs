using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.Application.Commons.Services;
using Audiochan.Application.Features.Auth.Commands.Login;
using Audiochan.Application.Features.Auth.Commands.Register;
using Audiochan.Application.Features.Auth.Queries.GetCurrentUser;
using MediatR;
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
        private readonly IMediator _mediator;
        private readonly IDateTimeProvider _dateTime;

        public AuthController(IMediator mediator, IDateTimeProvider dateTime)
        {
            _mediator = mediator;
            _dateTime = dateTime;
        }

        [HttpPost("login", Name = "Login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [SwaggerOperation(
            Summary = "Login using your credentials.",
            OperationId = "Login",
            Tags = new[] {"auth"}
        )]
        public async Task<ActionResult<CurrentUserDto>> Login([FromBody] LoginCommand command,
            CancellationToken cancellationToken)
        {
            var loginResult = await _mediator.Send(command, cancellationToken);
            
            if (!loginResult.IsSuccess)
            {
                return loginResult.ReturnErrorResponse();
            }

            var user = loginResult.Data!;

            Claim[] claims = {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties { ExpiresUtc = _dateTime.Now.AddDays(14) };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
            HttpContext.User = principal;

            return Ok(user);
        }

        [HttpPost("register", Name = "CreateAccount")]
        [ProducesResponseType(200)]
        [ProducesResponseType(422)]
        [SwaggerOperation(
            Summary = "Create an account.",
            Description = "Once successful, you can use the login endpoint to obtain access and refresh tokens.",
            OperationId = "CreateAccount",
            Tags = new[] {"auth"}
        )]
        public async Task<IActionResult> Register([FromBody] CreateUserCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
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