﻿using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Models;
using Audiochan.Core.Audios;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Users;
using Audiochan.Core.Users.GetFollowers;
using Audiochan.Core.Users.GetFollowings;
using Audiochan.Core.Users.GetProfile;
using Audiochan.Core.Users.GetUserAudios;
using Audiochan.Core.Users.GetUserFavoriteAudios;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// ReSharper disable RouteTemplates.ActionRoutePrefixCanBeExtractedToControllerRoute

namespace Audiochan.API.Controllers
{
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{username}", Name = "GetProfile")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [SwaggerOperation(Summary = "Return user's profile.", OperationId = "GetProfile", Tags = new[] {"users"})]
        public async Task<ActionResult<ProfileDto>> GetUser(string username, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetProfileQuery(username), cancellationToken);

            return result != null
                ? Ok(result)
                : NotFound(ErrorApiResponse.NotFound("User was not found."));
        }

        [HttpGet("{username}/audios", Name = "GetUserAudios")]
        [ProducesResponseType(200)]
        [SwaggerOperation(Summary = "Return a list of the user's audios.", OperationId = "GetUserAudios",
            Tags = new[] {"users"})]
        public async Task<ActionResult<PagedListDto<AudioDto>>> GetUserAudios(string username, 
            [FromQuery] OffsetPaginationQueryParams paginationQueryParams,
            CancellationToken cancellationToken)
        {
            var list = await _mediator.Send(new GetUsersAudioQuery(username)
            {
                Offset = paginationQueryParams.Offset,
                Size = paginationQueryParams.Size
            }, cancellationToken);

            return Ok(list);
        }

        [HttpGet("{username}/favorite/audios", Name = "GetUserFavoriteAudios")]
        [ProducesResponseType(200)]
        [SwaggerOperation(
            Summary = "Returns a list of user's favorite audios.",
            OperationId = "GetUsersFavoriteAudios",
            Tags=new []{"users"})]
        public async Task<IActionResult> GetUserFavoriteAudios(string username,
            [FromQuery] OffsetPaginationQueryParams paginationQueryParams, CancellationToken cancellationToken)
        {
            var list = await _mediator.Send(new GetUserFavoriteAudiosQuery
            {
                Username = username,
                Offset = paginationQueryParams.Offset,
                Size = paginationQueryParams.Size
            }, cancellationToken);
            return Ok(list);
        }

        [HttpGet("{username}/followers", Name = "GetUserFollowers")]
        [ProducesResponseType(200)]
        [SwaggerOperation(Summary = "Return a list of the user's followers.", OperationId = "GetUserFollowers",
            Tags = new[] {"users"})]
        public async Task<ActionResult<PagedListDto<UserDto>>> GetFollowers(string username, 
            [FromQuery] OffsetPaginationQueryParams paginationQueryParams, 
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetUserFollowersQuery(username)
            {
                Offset = paginationQueryParams.Offset,
                Size = paginationQueryParams.Size
            }, cancellationToken));
        }

        [HttpGet("{username}/followings", Name = "GetUserFollowings")]
        [ProducesResponseType(200)]
        [SwaggerOperation(Summary = "Return a list of the user's followings.", OperationId = "GetUserFollowings",
            Tags = new[] {"users"})]
        public async Task<ActionResult<PagedListDto<UserDto>>> GetFollowings(string username, 
            [FromQuery] OffsetPaginationQueryParams paginationQueryParams, 
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetUserFollowingsQuery
            {
                Username = username,
                Offset = paginationQueryParams.Offset,
                Size = paginationQueryParams.Size
            }, cancellationToken));
        }
    }
}