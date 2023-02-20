using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Dtos;
using Audiochan.Common.Exceptions;
using Audiochan.Common.Extensions;
using Audiochan.Core.Features.Users.Commands.SetFollow;
using Audiochan.Core.Features.Users.Commands.UpdateEmail;
using Audiochan.Core.Features.Users.Commands.UpdatePassword;
using Audiochan.Core.Features.Users.Commands.UpdatePicture;
using Audiochan.Core.Features.Users.Commands.UpdateProfile;
using Audiochan.Core.Features.Users.Commands.UpdateUsername;
using Audiochan.Core.Features.Users.Dtos;
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
    public async Task<UserDto> UpdateUserAsync(
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
    public async Task<ImageUploadResponse> UpdateUserPictureAsync(
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
    public async Task<ImageUploadResponse> RemoveUserPictureAsync(
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
    [Error(typeof(DuplicateUserNameException))]
    [Error(typeof(ValidationException))]
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
    [Error(typeof(PasswordDoesNotMatchException))]
    [Error(typeof(ValidationException))]
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