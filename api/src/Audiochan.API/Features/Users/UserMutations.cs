using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Users.Errors;
using Audiochan.API.GraphQL.Errors;
using Audiochan.Core.Entities;
using Audiochan.Core.Exceptions;
using Audiochan.Core.Features.Users.Commands;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Shared.Extensions;
using HotChocolate.Authorization;
using HotChocolate.Language;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using MediatR;

namespace Audiochan.API.Features.Users;

[ExtendObjectType(OperationType.Mutation)]
public class UserMutations
{
    [Authorize]
    [Error(typeof(UserNotFoundError))]
    public async Task<UserDto> UpdateUserAsync(
        [ID(nameof(UserDto))] long id,
        string? displayName,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        if (userId != id)
            throw new UnauthorizedAccessException();
        var command = new UpdateProfileCommand(id, displayName);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            user => user,
            _ => throw new EntityNotFoundException<User, long>(id),
            _ => throw new EntityNotFoundException<User, long>(id));
    }
    
    [Authorize]
    [UseMutationConvention(PayloadFieldName = "url")]
    [UseValidationError]
    [Error(typeof(UserNotFoundError))]
    public async Task<string> UpdateUserPictureAsync(
        [ID(nameof(UserDto))] long id,
        string data,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        if (userId != id)
            throw new UnauthorizedAccessException();
        var command = new UpdateUserPictureCommand(id, data);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            url => url,
            _ => throw new EntityNotFoundException<User, long>(id));
    }
    
    [Authorize]
    [UseMutationConvention(PayloadFieldName = "userPictureRemoved")]
    [UseValidationError]
    [Error(typeof(UserNotFoundError))]
    public async Task<bool> RemoveUserPictureAsync(
        [ID(nameof(UserDto))] long id,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        if (userId != id)
        {
            throw new UnauthorizedAccessException();
        }
        var command = new UpdateUserPictureCommand(id, null);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            string.IsNullOrEmpty,
            _ => throw new EntityNotFoundException<User, long>(id));
    }
    
    [Authorize]
    [UseMutationConvention(PayloadFieldName = "userUpdated")]
    [Error(typeof(IdentityError))]
    [Error(typeof(UserNotFoundError))]
    public async Task<bool> UpdateUserNameAsync(
        [ID(nameof(UserDto))] long id,
        string newUsername,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var currentUserId = claimsPrincipal.GetUserId();
        if (id != currentUserId)
        {
            throw new UnauthorizedAccessException();
        }
        var command = new UpdateUsernameCommand(id, newUsername);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            _ => true,
            _ => throw new EntityNotFoundException<User, long>(id),
            err => throw new AggregateException(err.Errors.Select(e => new IdentityException(e))));
    }
    
    [Authorize]
    [UseMutationConvention(PayloadFieldName = "passwordUpdated")]
    [Error(typeof(IdentityError))]
    public async Task<bool> UpdatePasswordAsync(
        string newPassword,
        string currentPassword,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePasswordCommand(newPassword, currentPassword, claimsPrincipal);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            _ => true,
            _ => throw new UnauthorizedAccessException(),
            id => throw new AggregateException(id.Errors.Select(e => new IdentityException(e))));
    }
    
    [Authorize]
    [UseMutationConvention(PayloadFieldName = "emailUpdated")]
    [Error(typeof(IdentityError))]
    public async Task<bool> UpdateEmailAsync(
        string email,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new UpdateEmailCommand(userId, email);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            _ => true,
            _ => throw new UnauthorizedAccessException(),
            err => throw new AggregateException(err.Errors.Select(e => new IdentityException(e))));
    }

    [Authorize]
    [UseMutationConvention(PayloadFieldName = "followed")]
    [Error(typeof(UserNotFoundError))]
    [Error(typeof(CannotFollowYourselfError))]
    public async Task<bool> FollowUserAsync(
        [ID(nameof(UserDto))] long id,
        IMediator mediator, 
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var currentUserId = claimsPrincipal.GetUserId();
        var command = new SetFollowCommand(id, currentUserId, true);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            res => res,
            _ => throw new EntityNotFoundException<User, long>(id),
            _ => throw new CannotFollowYourselfException());
    }
    
    [Authorize]
    [UseMutationConvention(PayloadFieldName = "followed")]
    [Error(typeof(UserNotFoundError))]
    [Error(typeof(CannotFollowYourselfError))]
    public async Task<bool> UnfollowUserAsync(
        [ID(nameof(UserDto))] long id,
        IMediator mediator, 
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var currentUserId = claimsPrincipal.GetUserId();
        var command = new SetFollowCommand(id, currentUserId, false);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            res => res,
            _ => throw new EntityNotFoundException<User, long>(id),
            _ => throw new CannotFollowYourselfException());
    }
}