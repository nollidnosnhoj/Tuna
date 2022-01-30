﻿using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.Application.Commons.Dtos.Requests;
using Audiochan.Application.Commons.Dtos.Responses;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Commons.Interfaces;
using Audiochan.Application.Commons.Services;
using Audiochan.Application.Features.Auth.Queries.GetCurrentUser;
using Audiochan.Application.Features.Users.Commands.RemovePicture;
using Audiochan.Application.Features.Users.Commands.UpdateEmail;
using Audiochan.Application.Features.Users.Commands.UpdatePassword;
using Audiochan.Application.Features.Users.Commands.UpdatePicture;
using Audiochan.Application.Features.Users.Commands.UpdateProfile;
using Audiochan.Application.Features.Users.Commands.UpdateUsername;
using Audiochan.Application.Features.Users.Queries.GetProfile;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public MeController(ICurrentUserService currentUserService, IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
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
                : Unauthorized();
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
        public async Task<ActionResult<ProfileDto>> UpdateUser([FromBody] UpdateProfileRequest request,
            CancellationToken cancellationToken)
        {
            var command = UpdateProfileCommand.FromRequest(_currentUserId, request);
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.Succeeded) return ((IErrorResult) result).ToErrorObjectResult();
            return Ok(_mapper.Map<ProfileDto>(result.Data));
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
            return result.ToObjectResult(Ok);
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
            return result.ToObjectResult(Ok);
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
            return result.ToObjectResult(Ok);
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
        public async Task<ActionResult<ImageUploadResponse>> AddPicture([FromBody] ImageUploadRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateUserPictureCommand(_currentUserId, request.Data);
            var result = await _mediator.Send(command, cancellationToken);
            return result.ToObjectResult(Ok);
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
            return result.ToObjectResult(NoContent);
        }
    }
}