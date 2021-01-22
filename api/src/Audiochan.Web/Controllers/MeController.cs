using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class MeController : ControllerBase
    {
        private readonly IAudioService _audioService;
        private readonly IUserService _userService;
        private readonly IFollowerService _followerService;
        private readonly long _currentUserId;

        public MeController(ICurrentUserService currentUserService, IAudioService audioService, 
            IUserService userService, IFollowerService followerService)
        {
            _audioService = audioService;
            _userService = userService;
            _followerService = followerService;
            _currentUserId = currentUserService.GetUserId();
        }

        [HttpHead(Name="IsAuthenticated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "Check if authenticated",
            Description = "Requires authentication.",
            OperationId = "IsAuthenticated",
            Tags = new []{"me"}
        )]
        public IActionResult IsAuthenticated()
        {
            return Ok();
        }

        [HttpGet(Name="GetAuthenticatedUser")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CurrentUserViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Returns information about authenticated user",
            Description = "Requires authentication.",
            OperationId = "GetAuthenticatedUser",
            Tags = new []{"me"}
        )]
        public async Task<IActionResult> GetAuthenticatedUser(CancellationToken cancellationToken)
        {
            var result = await _userService.GetCurrentUser(_currentUserId, cancellationToken);
            return result.IsSuccess 
                ? Ok(result.Data) 
                : result.ReturnErrorResponse();
        }

        [HttpGet("feed", Name="GetAuthenticatedUserFeed")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedList<AudioListViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "Returns a list of tracks uploaded by authenticated user's followings.",
            Description = "Requires authentication.",
            OperationId = "GetAuthenticatedUserFeed",
            Tags = new []{"me"}
        )]
        public async Task<IActionResult> GetAuthenticatedUserFeed([FromQuery] PaginationQuery query, 
            CancellationToken cancellationToken)
        {
            var result = await _audioService.GetFeed(_currentUserId, query, cancellationToken);
            return Ok(result);
        }
        
        [HttpHead("followings/{username}", Name="CheckIfUserFollowedUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Check if the authenticated user follows a user",
            Description = "Requires authentication.",
            OperationId = "CheckIfUserFollowedUser",
            Tags = new []{"me"}
        )]
        public async Task<IActionResult> IsFollow(string username, CancellationToken cancellationToken)
        {
            return await _followerService.CheckFollowing(_currentUserId, username, cancellationToken)
                ? Ok()
                : NotFound();
        }

        [HttpPut("followings/{username}", Name="FollowUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [SwaggerOperation(
            Summary = "Follow a user",
            Description = "Requires authentication.",
            OperationId = "FollowUser",
            Tags = new []{"me"}
        )]
        public async Task<IActionResult> Follow(string username, CancellationToken cancellationToken)
        {
            var result = await _followerService.Follow(_currentUserId, username.ToLower(), cancellationToken);
            return result.IsSuccess 
                ? Ok() 
                : result.ReturnErrorResponse();
        }

        [HttpDelete("followings/{username}", Name="UnfollowUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [SwaggerOperation(
            Summary = "Unfollow a user",
            Description = "Requires authentication.",
            OperationId = "UnfollowUser",
            Tags = new []{"me"}
        )]
        public async Task<IActionResult> Unfollow(string username, CancellationToken cancellationToken)
        {
            var result = await _followerService.Unfollow(_currentUserId, username.ToLower(), cancellationToken);
            return result.IsSuccess 
                ? NoContent() 
                : result.ReturnErrorResponse();
        }

        [HttpPut(Name="UpdateUser")]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [SwaggerOperation(
            Summary = "Updates authenticated user.",
            Description = "Requires authentication.",
            OperationId = "UpdateUser",
            Tags = new []{"me"}
        )]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDetailsRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateUser(_currentUserId, request, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpPatch("settings/username", Name="UpdateUsername")]
        [SwaggerOperation(
            Summary = "Updates authenticated user's username.",
            Description = "Requires authentication.",
            OperationId = "UpdateUsername",
            Tags = new []{"me"}
        )]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> ChangeUsername([FromBody] UpdateUsernameRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateUsername(_currentUserId, request.Username!, cancellationToken);
            return result.IsSuccess 
                ? Ok() 
                : result.ReturnErrorResponse();
        }

        [HttpPatch("settings/email", Name="UpdateEmail")]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [SwaggerOperation(
            Summary = "Updates authenticated user's email.",
            Description = "Requires authentication.",
            OperationId = "UpdateEmail",
            Tags = new []{"me"}
        )]
        public async Task<IActionResult> ChangeEmail([FromBody] UpdateEmailRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateEmail(_currentUserId, request.Email!, cancellationToken);
            return result.IsSuccess 
                ? Ok() 
                : result.ReturnErrorResponse();
        }

        [HttpPatch("settings/password", Name="UpdatePassword")]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [SwaggerOperation(
            Summary = "Updates authenticated user's password.",
            Description = "Requires authentication.",
            OperationId = "UpdatePassword",
            Tags = new []{"me"}
        )]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _userService.UpdatePassword(_currentUserId, request, cancellationToken);
            return result.IsSuccess 
                ? Ok() 
                : result.ReturnErrorResponse();
        }
    }
}