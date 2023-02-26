using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Errors;
using Audiochan.API.Features.Users.Errors;
using Audiochan.Common.Extensions;
using Audiochan.Core.Features.Upload.Models;
using Audiochan.Core.Features.Users.Commands;
using Audiochan.Core.Features.Users.Extensions;
using Audiochan.Core.Features.Users.Models;
using HotChocolate.Authorization;
using HotChocolate.Language;
using HotChocolate.Types;
using MediatR;

namespace Audiochan.API.Features.Users;

[ExtendObjectType(OperationType.Mutation)]
public class UserMutations
{
    [Authorize]
    [Error(typeof(UnauthorizedError))]
    public async Task<UserViewModel> UpdateUserAsync(
        string? displayName,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProfileCommand(displayName, claimsPrincipal);
        return await mediator.Send(command, cancellationToken);
    }
    
    [Authorize]
    [Error(typeof(UnauthorizedError))]
    [Error(typeof(ValidationError))]
    public async Task<ImageUploadResult> UpdateUserPictureAsync(
        long userId,
        string data,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var currentUserId = claimsPrincipal.GetUserId();
        if (userId != currentUserId)
        {
            throw new UnauthorizedAccessException();
        }
        var command = new UpdateUserPictureCommand(userId, data);
        return await mediator.Send(command, cancellationToken);
    }
    
    [Authorize]
    [Error(typeof(UnauthorizedError))]
    [Error(typeof(ValidationError))]
    public async Task<ImageUploadResult> RemoveUserPictureAsync(
        long userId,
        string data,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var currentUserId = claimsPrincipal.GetUserId();
        if (userId != currentUserId)
        {
            throw new UnauthorizedAccessException();
        }
        var command = new UpdateUserPictureCommand(userId, null);
        return await mediator.Send(command, cancellationToken);
    }
    
    [Authorize]
    [Error(typeof(UnauthorizedError))]
    [Error(typeof(UserIdentityError))]
    public async Task<bool> UpdateUserNameAsync(
        long userId,
        string userName,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var currentUserId = claimsPrincipal.GetUserId();
        if (userId != currentUserId)
        {
            throw new UnauthorizedAccessException();
        }
        var command = new UpdateUsernameCommand(userId, userName);
        var result = await mediator.Send(command, cancellationToken);
        result.EnsureSuccessful();
        return result.Succeeded;
    }
    
    [Authorize]
    [Error(typeof(UnauthorizedError))]
    [Error(typeof(UserIdentityError))]
    public async Task<bool> UpdatePasswordAsync(
        string currentPassword,
        string newPassword,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePasswordCommand(newPassword, currentPassword, claimsPrincipal);
        var result = await mediator.Send(command, cancellationToken);
        result.EnsureSuccessful();
        return result.Succeeded;
    }
    
    [Authorize]
    [Error(typeof(UnauthorizedError))]
    [Error(typeof(UserIdentityError))]
    public async Task<bool> UpdateEmailAsync(
        long userId,
        string email,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var currentUserId = claimsPrincipal.GetUserId();
        if (userId != currentUserId)
        {
            throw new UnauthorizedAccessException();
        }
        var command = new UpdateEmailCommand(userId, email);
        var result = await mediator.Send(command, cancellationToken);
        result.EnsureSuccessful();
        return result.Succeeded;
    }
    
    

    [Authorize]
    [Error<UserNotFoundError>]
    [Error<FollowError>]
    public async Task<bool> FollowUserAsync(long userId, 
        IMediator mediator, 
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var currentUserId = claimsPrincipal.GetUserId();
        var command = new SetFollowCommand(userId, currentUserId, true);
        return await mediator.Send(command, cancellationToken);
    }
    
    [Authorize]
    [Error<UserNotFoundError>]
    [Error<FollowError>]
    public async Task<bool> UnfollowUserAsync(long userId, 
        IMediator mediator, 
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var currentUserId = claimsPrincipal.GetUserId();
        var command = new SetFollowCommand(userId, currentUserId, false);
        return await mediator.Send(command, cancellationToken);
    }
}