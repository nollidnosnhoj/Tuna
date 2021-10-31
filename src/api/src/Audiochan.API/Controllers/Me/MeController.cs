using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.API.Models;
using Audiochan.Core.Auth;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers.Me
{
    [Area("me")]
    [Route("[area]")]
    [Authorize]
    [ProducesResponseType(401)]
    public class MeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly long _currentUserId;

        public MeController(ICurrentUserService currentUserService, IMediator mediator)
        {
            _mediator = mediator;
            currentUserService.User.TryGetUserId(out _currentUserId);
        }

        [HttpHead(Name = "IsAuthenticated")]
        [ProducesResponseType(200)]
        [SwaggerOperation(
            Summary = "Check if authenticated",
            Description = "Requires authentication.",
            OperationId = "IsAuthenticated",
            Tags = new[] {"me"}
        )]
        public IActionResult IsAuthenticated()
        {
            return Ok();
        }

        [HttpGet(Name = "GetAuthenticatedUser")]
        [ProducesResponseType(200)]
        [Produces("application/json")]
        [SwaggerOperation(
            Summary = "Returns information about authenticated user",
            Description = "Requires authentication.",
            OperationId = "GetYourInfo",
            Tags = new[] {"me"}
        )]
        public async Task<ActionResult<CurrentUserDto>> GetYourInfo(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCurrentUserQuery(_currentUserId), cancellationToken);
            return result != null
                ? Ok(result)
                : Unauthorized(ErrorApiResponse.Unauthorized());
        }

        [HttpPut(Name = "UpdateUser")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [SwaggerOperation(
            Summary = "Updates authenticated user.",
            Description = "Requires authentication.",
            OperationId = "UpdateUser",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateProfileRequest request,
            CancellationToken cancellationToken)
        {
            var command = UpdateProfileCommand.FromRequest(_currentUserId, request);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpPatch("username", Name = "UpdateUsername")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(422)]
        [SwaggerOperation(
            Summary = "Updates authenticated user's username.",
            Description = "Requires authentication.",
            OperationId = "UpdateUsername",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> ChangeUsername([FromBody] UpdateUsernameRequest request,
            CancellationToken cancellationToken)
        {
            var command = UpdateUsernameCommand.FromRequest(_currentUserId, request);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpPatch("email", Name = "UpdateEmail")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(422)]
        [SwaggerOperation(
            Summary = "Updates authenticated user's email.",
            Description = "Requires authentication.",
            OperationId = "UpdateEmail",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> ChangeEmail([FromBody] UpdateEmailRequest request,
            CancellationToken cancellationToken)
        {
            var command = UpdateEmailCommand.FromRequest(_currentUserId, request);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpPatch("password", Name = "UpdatePassword")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(422)]
        [SwaggerOperation(
            Summary = "Updates authenticated user's password.",
            Description = "Requires authentication.",
            OperationId = "UpdatePassword",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordRequest request,
            CancellationToken cancellationToken)
        {
            var command = UpdatePasswordCommand.FromRequest(_currentUserId, request);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpPatch("picture")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [SwaggerOperation(
            Summary = "Add picture to user.",
            Description = "Requires authentication.",
            OperationId = "AddUserPicture",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> AddPicture([FromBody] ImageUploadRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateUserPictureCommand(_currentUserId, request.Data);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Data)
                : result.ReturnErrorResponse();
        }
        
        [HttpDelete("picture")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [SwaggerOperation(
            Summary = "Remove picture to user.",
            Description = "Requires authentication.",
            OperationId = "RemoveUserPicture",
            Tags = new[] {"me"}
        )]
        public async Task<ActionResult> RemovePicture(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RemoveUserPictureCommand(_currentUserId), cancellationToken);
            
            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }
    }
}