using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Errors;
using Audiochan.API.Features.Audios.Payloads;
using Audiochan.API.Features.Users.Errors;
using Audiochan.API.Features.Users.Inputs;
using Audiochan.API.Features.Users.Payloads;
using Audiochan.Common.Extensions;
using Audiochan.Core.Features.Users.Commands;
using HotChocolate.Authorization;
using HotChocolate.Language;
using HotChocolate.Types;
using MediatR;

namespace Audiochan.API.Features.Users;

[ExtendObjectType(OperationType.Mutation)]
public class UserMutations
{
    [Authorize]
    public async Task<UserPayload> UpdateUserAsync(
        UpdateUserInput input,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        if (userId != input.UserId)
            throw new UnauthorizedAccessException();
        var command = new UpdateProfileCommand(input.UserId, input.DisplayName);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            user => new UserPayload(user),
            _ => throw new UnauthorizedAccessException(),
            _ => throw new UnauthorizedAccessException());
    }
    
    [Authorize]
    [UseValidationError]
    public async Task<UpdatePicturePayload> UpdateUserPictureAsync(
        UpdateUserPictureInput input,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        if (userId != input.UserId)
            throw new UnauthorizedAccessException();
        var command = new UpdateUserPictureCommand(input.UserId, input.Data);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            res => new UpdatePicturePayload(res),
            _ => throw new UnauthorizedAccessException());
    }
    
    [Authorize]
    [UseValidationError]
    public async Task<RemovePicturePayload> RemoveUserPictureAsync(
        RemoveUserPictureInput input,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        if (userId != input.UserId)
        {
            throw new UnauthorizedAccessException();
        }
        var command = new UpdateUserPictureCommand(input.UserId, null);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            _ => new RemovePicturePayload(),
            _ => new RemovePicturePayload(new UserNotFoundError(input.UserId)));
    }
    
    [Authorize]
    public async Task<UpdateUsernamePayload> UpdateUserNameAsync(
        UpdateUsernameInput input,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var currentUserId = claimsPrincipal.GetUserId();
        if (input.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException();
        }
        var command = new UpdateUsernameCommand(input.UserId, input.NewUsername);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            _ => new UpdateUsernamePayload(),
            _ => throw new UnauthorizedAccessException(),
            idRes => new UpdateUsernamePayload(idRes.Errors));
    }
    
    [Authorize]
    public async Task<UpdatePasswordPayload> UpdatePasswordAsync(
        UpdatePasswordInput input,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePasswordCommand(input.NewPassword, input.CurrentPassword, claimsPrincipal);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            _ => new UpdatePasswordPayload(),
            _ => throw new UnauthorizedAccessException(),
            id => new UpdatePasswordPayload(id.Errors));
    }
    
    [Authorize]
    public async Task<UpdateEmailPayload> UpdateEmailAsync(
        UpdateEmailInput input,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new UpdateEmailCommand(userId, input.Email);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            _ => new UpdateEmailPayload(),
            _ => throw new UnauthorizedAccessException(),
            id => new UpdateEmailPayload(id.Errors));
    }

    [Authorize]
    public async Task<FollowUserPayload> FollowUserAsync(
        FollowUserInput input,
        IMediator mediator, 
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var currentUserId = claimsPrincipal.GetUserId();
        var command = new SetFollowCommand(input.UserId, currentUserId, true);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            res => new FollowUserPayload(res),
            _ => new FollowUserPayload(new UserNotFoundError(input.UserId)),
            err => new FollowUserPayload(err));
    }
    
    [Authorize]
    public async Task<FollowUserPayload> UnfollowUserAsync(
        FollowUserInput input,
        IMediator mediator, 
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var currentUserId = claimsPrincipal.GetUserId();
        var command = new SetFollowCommand(input.UserId, currentUserId, false);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            res => new FollowUserPayload(res),
            _ => new FollowUserPayload(new UserNotFoundError(input.UserId)),
            err => new FollowUserPayload(err));
    }
}