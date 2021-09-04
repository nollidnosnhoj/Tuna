using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.Core.Auth;
using Audiochan.Core.Auth.Login;
using Audiochan.Core.Auth.Refresh;
using Audiochan.Core.Auth.Register;
using Audiochan.Core.Auth.Revoke;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers
{
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login", Name = "Login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [SwaggerOperation(
            Summary = "Obtain access and refresh token using your login credentials.",
            OperationId = "Login",
            Tags = new[] {"auth"}
        )]
        public async Task<ActionResult<AuthResultViewModel>> Login([FromBody] LoginCommand command,
            CancellationToken cancellationToken)
        {
            var authResult = await _mediator.Send(command, cancellationToken);
            return authResult.IsSuccess
                ? Ok(authResult.Data)
                : authResult.ReturnErrorResponse();
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

        [HttpPost("refresh", Name = "RefreshAccessToken")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [SwaggerOperation(
            Summary = "Refresh access token using valid refresh token.",
            Description =
                "Once successful, you will also get a new refresh token, and the previous token will be invalid.",
            OperationId = "RefreshAccessToken",
            Tags = new[] {"auth"}
        )]
        public async Task<ActionResult<AuthResultViewModel>> Refresh([FromBody] RefreshTokenCommand command,
            CancellationToken cancellationToken)
        {
            var authResult = await _mediator.Send(command, cancellationToken);
            return authResult.IsSuccess
                ? Ok(authResult.Data)
                : authResult.ReturnErrorResponse();
        }

        [HttpPost("revoke", Name = "RevokeRefreshToken")]
        [ProducesResponseType(200)]
        [SwaggerOperation(
            Summary = "Revoke a refresh token",
            OperationId = "RevokeRefreshToken",
            Tags = new[] {"auth"}
        )]
        public async Task<ActionResult> Revoke([FromBody] RevokeTokenCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }
    }
}