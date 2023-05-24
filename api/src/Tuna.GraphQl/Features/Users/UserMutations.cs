using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Authorization;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using MediatR;
using Tuna.Application.Exceptions;
using Tuna.Application.Features.Uploads.Commands;
using Tuna.Application.Features.Uploads.Models;
using Tuna.Application.Features.Users.Commands;
using Tuna.Application.Features.Users.Exceptions;
using Tuna.Application.Features.Users.Models;
using Tuna.Domain.Entities;
using Tuna.Domain.Exceptions;
using Tuna.GraphQl.GraphQL.Errors;
using Tuna.Shared.Extensions;

namespace Tuna.GraphQl.Features.Users;

[MutationType]
public static class UserMutations
{
    [Authorize]
    [Error(typeof(UserNotFoundException))]
    public static async Task<UserDto> UpdateUserAsync(
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
            err => throw err);
    }


    [Authorize]
    [UseMutationConvention(PayloadFieldName = "url")]
    [UseValidationError]
    public static async Task<CreateUploadResult> CreateUserPictureAsync(
        string fileName,
        long fileSize,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new CreateImageUploadCommand(fileName, fileSize, UploadImageType.User, userId);
        return await mediator.Send(command, cancellationToken);
    }

    [Authorize]
    [UseMutationConvention(PayloadFieldName = "url")]
    [UseValidationError]
    [Error(typeof(UserNotFoundException))]
    public static async Task<string?> UpdateUserPictureAsync(
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
            err => throw err);
    }

    [Authorize]
    [UseMutationConvention(PayloadFieldName = "userPictureRemoved")]
    [UseValidationError]
    [Error(typeof(UserNotFoundException))]
    public static async Task<bool> RemoveUserPictureAsync(
        [ID(nameof(UserDto))] long id,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        if (userId != id) throw new UnauthorizedAccessException();
        var command = new UpdateUserPictureCommand(id, null);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            string.IsNullOrEmpty,
            err => throw err);
    }

    [Authorize]
    [UseMutationConvention(PayloadFieldName = "userUpdated")]
    [Error(typeof(UserIdentityException))]
    [Error(typeof(UserNotFoundException))]
    public static async Task<bool> UpdateUserNameAsync(
        [ID(nameof(UserDto))] long id,
        string newUsername,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var currentUserId = claimsPrincipal.GetUserId();
        if (id != currentUserId) throw new UnauthorizedAccessException();
        var command = new UpdateUsernameCommand(id, newUsername);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            _ => true,
            err => throw err);
    }

    [Authorize]
    [UseMutationConvention(PayloadFieldName = "passwordUpdated")]
    [Error(typeof(UserIdentityException))]
    public static async Task<bool> UpdatePasswordAsync(
        string newPassword,
        string currentPassword,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new UpdatePasswordCommand(userId, newPassword, currentPassword);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            _ => true,
            err => throw err);
    }

    [Authorize]
    [UseMutationConvention(PayloadFieldName = "emailUpdated")]
    [Error(typeof(UserIdentityException))]
    public static async Task<bool> UpdateEmailAsync(
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
            err => throw err);
    }

    [Authorize]
    [UseMutationConvention(PayloadFieldName = "followed")]
    [Error(typeof(UserNotFoundException))]
    [Error(typeof(FollowingException))]
    public static async Task<bool> FollowUserAsync(
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
            err => throw err);
    }

    [Authorize]
    [UseMutationConvention(PayloadFieldName = "followed")]
    [Error(typeof(UserNotFoundException))]
    [Error(typeof(FollowingException))]
    public static async Task<bool> UnfollowUserAsync(
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
            err => throw err);
    }
}