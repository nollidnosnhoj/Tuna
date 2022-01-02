using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Features.Auth.Commands.Login;
using Audiochan.Application.Features.Auth.Commands.Register;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.Server.Controllers
{
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;

        public AuthController(IMediator mediator, IAuthService authService)
        {
            _mediator = mediator;
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
        public async Task<ActionResult<CurrentUserDto>> Login([FromBody] LoginCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(command, cancellationToken);

            await _authService.LoginAsync(user, cancellationToken);

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
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpPost("logout")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Authorize]
        [SwaggerOperation(Summary = "Logout user", OperationId = "Logout", Tags = new[]{"auth"})]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            await _authService.LogoutAsync(cancellationToken);
            return Ok();
        }
    }
}