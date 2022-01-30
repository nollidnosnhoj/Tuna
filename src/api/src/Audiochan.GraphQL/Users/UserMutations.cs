using System.Security.Claims;
using Audiochan.Application.Commons.Dtos.Responses;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Users.Commands.RemovePicture;
using Audiochan.Application.Features.Users.Commands.SetFollow;
using Audiochan.Application.Features.Users.Commands.UpdateEmail;
using Audiochan.Application.Features.Users.Commands.UpdatePassword;
using Audiochan.Application.Features.Users.Commands.UpdatePicture;
using Audiochan.Application.Features.Users.Commands.UpdateProfile;
using Audiochan.Application.Features.Users.Commands.UpdateUsername;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Domain.Entities;
using Audiochan.GraphQL.Common.Errors;
using Audiochan.GraphQL.Users.Errors;
using HotChocolate.AspNetCore.Authorization;
using MediatR;

namespace Audiochan.GraphQL.Users;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class UserMutations
{
    [UseMutationConvention]
    [Authorize]
    public async Task<UserDto> UpdateProfile(
        string? displayName,
        string? about,
        string? website,
        ClaimsPrincipal claimsPrincipal,
        [Service] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new UpdateProfileCommand(userId, displayName, about, website);
        return await mediator.Send(command, cancellationToken);
    }

    [UseMutationConvention(PayloadFieldName = "response")]
    [Authorize]
    [Error(typeof(Forbidden))]
    public async Task<ImageUploadResponse> UpdateUserPicture(
        string data,
        ClaimsPrincipal claimsPrincipal,
        [Service] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new UpdateUserPictureCommand(userId, data);
        return await mediator.Send(command, cancellationToken);
    }
    
    [UseMutationConvention(PayloadFieldName = "success")]
    [Authorize]
    [Error(typeof(Forbidden))]
    public async Task<bool> RemoveUserPicture(
        ClaimsPrincipal claimsPrincipal,
        [Service] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new RemoveUserPictureCommand(userId);
        await mediator.Send(command, cancellationToken);
        return true;
    }

    [UseMutationConvention(PayloadFieldName = "success")]
    [Authorize]
    [Error(typeof(Forbidden))]
    [Error(typeof(UsernameTaken))]
    public async Task<bool> UpdateUsername(
        string username,
        ClaimsPrincipal claimsPrincipal,
        [Service] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new UpdateUsernameCommand(userId, username);
        await mediator.Send(command, cancellationToken);
        return true;
    }
    
    [UseMutationConvention(PayloadFieldName = "success")]
    [Authorize]
    [Error(typeof(Forbidden))]
    [Error(typeof(EmailTaken))]
    public async Task<bool> UpdateEmail(
        string email,
        ClaimsPrincipal claimsPrincipal,
        [Service] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new UpdateEmailCommand(userId, email);
        await mediator.Send(command, cancellationToken);
        return true;
    }
    
    [UseMutationConvention(PayloadFieldName = "success")]
    [Authorize]
    [Error(typeof(Forbidden))]
    [Error(typeof(UnmatchedPassword))]
    public async Task<bool> UpdatePassword(
        string currentPassword,
        string newPassword,
        ClaimsPrincipal claimsPrincipal,
        [Service] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new UpdatePasswordCommand(userId, currentPassword, newPassword);
        await mediator.Send(command, cancellationToken);
        return true;
    }

    [UseMutationConvention(PayloadFieldName = "success")]
    [Authorize]
    [Error(typeof(UserNotFound))]
    [Error(typeof(Forbidden))]
    public async Task<bool> Follow(
        [ID(nameof(User))] long targetUserId,
        ClaimsPrincipal claimsPrincipal,
        [Service] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var observerUserId = claimsPrincipal.GetUserId();
        var command = new SetFollowCommand(observerUserId, targetUserId, true);
        await mediator.Send(command, cancellationToken);
        return true;
    }
    
    [UseMutationConvention(PayloadFieldName = "success")]
    [Authorize]
    [Error(typeof(UserNotFound))]
    [Error(typeof(Forbidden))]
    public async Task<bool> Unfollow(
        [ID(nameof(User))] long targetUserId,
        ClaimsPrincipal claimsPrincipal,
        [Service] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var observerUserId = claimsPrincipal.GetUserId();
        var command = new SetFollowCommand(observerUserId, targetUserId, false);
        await mediator.Send(command, cancellationToken);
        return true;
    }
}