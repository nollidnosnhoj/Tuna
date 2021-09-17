using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.Core.Auth.GetCurrentUser;
using Audiochan.Core.Auth.Login;
using Audiochan.Core.Auth.Register;
using Audiochan.Core.Common.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers
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
            var loginResult = await _mediator.Send(command, cancellationToken);
            
            if (!loginResult.IsSuccess)
            {
                return loginResult.ReturnErrorResponse();
            }

            var user = loginResult.Data!;

            await _authService.SignIn(user);

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
    }
}