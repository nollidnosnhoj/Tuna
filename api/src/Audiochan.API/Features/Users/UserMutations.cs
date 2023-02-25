using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Exceptions;
using Audiochan.Common.Extensions;
using Audiochan.Core.Features.Auth.Exceptions;
using Audiochan.Core.Features.Upload.Models;
using Audiochan.Core.Features.Users;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Core.Features.Users.Exceptions;
using FluentValidation;
using HotChocolate.Authorization;
using HotChocolate.Language;
using HotChocolate.Types;
using MediatR;

namespace Audiochan.API.Features.Users;

[ExtendObjectType(OperationType.Mutation)]
public class UserMutations
{
    [Authorize]
    [Error(typeof(UnauthorizedException))]
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
    [Error(typeof(UnauthorizedException))]
    [Error(typeof(ValidationException))]
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
            throw new UnauthorizedException();
        }
        var command = new UpdateUserPictureCommand(userId, data);
        return await mediator.Send(command, cancellationToken);
    }
    
    [Authorize]
    [Error(typeof(UnauthorizedException))]
    [Error(typeof(ValidationException))]
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
            throw new UnauthorizedException();
        }
        var command = new UpdateUserPictureCommand(userId, null);
        return await mediator.Send(command, cancellationToken);
    }
    
    [Authorize]
    [Error(typeof(UnauthorizedException))]
    [Error(typeof(IdentityException))]
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
            throw new UnauthorizedException();
        }
        var command = new UpdateUsernameCommand(userId, userName);
        return await mediator.Send(command, cancellationToken);
    }
    
    [Authorize]
    [Error(typeof(UnauthorizedException))]
    [Error(typeof(IdentityException))]
    public async Task<bool> UpdatePasswordAsync(
        string currentPassword,
        string newPassword,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePasswordCommand(newPassword, currentPassword, claimsPrincipal);
        return await mediator.Send(command, cancellationToken);
    }
    
    [Authorize]
    [Error(typeof(UnauthorizedException))]
    [Error(typeof(IdentityException))]
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
            throw new UnauthorizedException();
        }
        var command = new UpdateEmailCommand(userId, email);
        return await mediator.Send(command, cancellationToken);
    }
    
    

    [Authorize]
    [Error<UserNotFoundException>]
    [Error<CannotFollowYourselfException>]
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
    [Error<UserNotFoundException>]
    [Error<CannotFollowYourselfException>]
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