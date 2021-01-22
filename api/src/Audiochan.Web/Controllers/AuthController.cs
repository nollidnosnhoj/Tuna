using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Auth.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Web.Extensions;
using Audiochan.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.Web.Controllers
{
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login", Name="Login")]
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Obtain access and refresh token using your login credentials.",
            OperationId = "Login",
            Tags = new []{ "auth" }
        )]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, 
            CancellationToken cancellationToken)
        {
            var authResult = await _authService.Login(request.Username!, request.Password!, cancellationToken);
            return authResult.IsSuccess 
                ? Ok(authResult.Data) 
                : authResult.ReturnErrorResponse();
        }
        
        [HttpPost("register", Name="CreateAccount")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [SwaggerOperation(
            Summary = "Create an account.",
            Description = "Once successful, you can use the login endpoint to obtain access and refresh tokens.",
            OperationId = "CreateAccount",
            Tags = new []{ "auth" }
        )]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _authService.Register(request, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpPost("refresh", Name="RefreshAccessToken")]
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Refresh access token using valid refresh token.",
            Description = "Once successful, you will also get a new refresh token, and the previous token will be invalid.",
            OperationId = "RefreshAccessToken",
            Tags = new []{"auth"}
        )]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request,
            CancellationToken cancellationToken)
        {
            var authResult = await _authService.Refresh(request.RefreshToken, cancellationToken);
            return authResult.IsSuccess 
                ? Ok(authResult.Data) 
                : authResult.ReturnErrorResponse();
        }

        [HttpPost("revoke", Name="RevokeRefreshToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Revoke a refresh token",
            OperationId = "RevokeRefreshToken",
            Tags = new []{"auth"}
        )]
        public async Task<IActionResult> Revoke([FromBody] RefreshTokenRequest request, 
            CancellationToken cancellationToken)
        {
            var result = await _authService.Revoke(request.RefreshToken, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }
    }
}