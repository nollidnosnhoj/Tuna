using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Auth.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, 
            CancellationToken cancellationToken)
        {
            var authResult = await _authService.Login(request.Username!, request.Password!, cancellationToken);

            return authResult.IsSuccess ? Ok(authResult.Data) : authResult.ReturnErrorResponse();
        }
        
        [HttpPost("register", Name="CreateAccount")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _authService.Register(request, cancellationToken);

            return result.IsSuccess
                ? StatusCode(StatusCodes.Status201Created)
                : result.ReturnErrorResponse();
        }

        [HttpPost("refresh", Name="RefreshAccessToken")]
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request,
            CancellationToken cancellationToken)
        {
            var authResult = await _authService.Refresh(request.RefreshToken, cancellationToken);

            return authResult.IsSuccess ? Ok(authResult.Data) : authResult.ReturnErrorResponse();
        }

        [HttpPost("revoke", Name="RevokeRefreshToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Revoke([FromBody] RefreshTokenRequest request, 
            CancellationToken cancellationToken)
        {
            var result = await _authService.Revoke(request.RefreshToken, cancellationToken);

            Response.Cookies.Delete("refreshToken");
            
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }
    }
}