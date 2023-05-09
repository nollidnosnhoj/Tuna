using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Authorization;
using HotChocolate.Language;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using MediatR;
using Tuna.Application.Exceptions;
using Tuna.Application.Features.Audios.Commands;
using Tuna.Application.Features.Audios.Models;
using Tuna.Application.Features.Uploads.Commands;
using Tuna.Application.Features.Uploads.Models;
using Tuna.Domain;
using Tuna.Domain.Entities;
using Tuna.Domain.Exceptions;
using Tuna.GraphQl.Features.Audios.Errors;
using Tuna.GraphQl.GraphQL.Errors;
using Tuna.Shared.Extensions;

namespace Tuna.GraphQl.Features.Audios;

[MutationType]
public static class AudioMutations
{
    [Authorize]
    [UseValidationError]
    public static async Task<CreateAudioResult> CreateAudioAsync(
        string fileName,
        long fileSize,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new CreateAudioCommand(fileName, fileSize, userId);
        return await mediator.Send(command, cancellationToken);
    }

    [Authorize]
    [UseValidationError]
    [Error(typeof(AudioNotFoundError))]
    [Error(typeof(AudioNotUploadedError))]
    public static async Task<AudioDto> PublishAudioAsync(
        [ID<AudioDto>] long id,
        decimal duration,
        string title,
        string? description,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new PublishAudioCommand(
            id,
            title,
            description ?? "",
            duration,
            claimsPrincipal.GetUserId());
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            audio => audio,
            _ => throw new EntityNotFoundException<Audio, long>(id),
            _ => throw new AudioNotUploadedException());
    }

    [Authorize]
    [UseValidationError]
    [Error(typeof(AudioNotFoundError))]
    public static async Task<AudioDto> UpdateAudioAsync(
        [ID(nameof(AudioDto))] long id,
        string? title,
        string? description,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAudioCommand(id, title, description);
        command.SetAuthenticated(claimsPrincipal);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            audio => audio,
            _ => throw new EntityNotFoundException<Audio, long>(id),
            _ => throw new EntityNotFoundException<Audio, long>(id));
    }

    [Authorize]
    [UseValidationError]
    [Error(typeof(AudioNotFoundError))]
    public static async Task<CreateUploadResult> CreateAudioPictureAsync(
        string fileName,
        long fileSize,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new CreateImageUploadCommand(fileName, fileSize, UploadImageType.Audio, userId);
        return await mediator.Send(command, cancellationToken);
    }

    [Authorize]
    [UseMutationConvention(PayloadFieldName = "url")]
    [UseValidationError]
    [Error(typeof(AudioNotFoundError))]
    public static async Task<string> UpdateAudioPictureAsync(
        [ID(nameof(AudioDto))] long id,
        string data,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAudioPictureCommand(id, data);
        command.SetAuthenticated(claimsPrincipal);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            res => res,
            _ => throw new EntityNotFoundException<Audio, long>(id),
            _ => throw new EntityNotFoundException<Audio, long>(id));
    }

    [Authorize]
    [UseMutationConvention(PayloadFieldName = "removed")]
    [Error(typeof(AudioNotFoundError))]
    public static async Task<bool> RemoveAudioAsync(
        [ID(nameof(AudioDto))] long id,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new RemoveAudioCommand(id);
        command.SetAuthenticated(claimsPrincipal);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            _ => true,
            _ => throw new EntityNotFoundException<Audio, long>(id),
            _ => throw new EntityNotFoundException<Audio, long>(id));
    }

    [Authorize]
    [UseMutationConvention(PayloadFieldName = "audioPictureRemoved")]
    [UseValidationError]
    [Error(typeof(AudioNotFoundError))]
    public static async Task<bool> RemoveAudioPictureAsync(
        [ID(nameof(AudioDto))] long id,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAudioPictureCommand(id, null);
        command.SetAuthenticated(claimsPrincipal);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            string.IsNullOrEmpty,
            _ => throw new EntityNotFoundException<Audio, long>(id),
            _ => throw new EntityNotFoundException<Audio, long>(id));
    }

    [Authorize]
    [UseMutationConvention(PayloadFieldName = "favorited")]
    [Error(typeof(AudioNotFoundError))]
    public static async Task<bool> FavoriteAudioAsync(
        [ID(nameof(AudioDto))] long id,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new SetFavoriteAudioCommand(id, userId, true);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            isFavorited => isFavorited,
            _ => throw new EntityNotFoundException<Audio, long>(id));
    }

    [Authorize]
    [UseMutationConvention(PayloadFieldName = "favorited")]
    [Error(typeof(AudioNotFoundError))]
    public static async Task<bool> UnfavoriteAudioAsync(
        [ID(nameof(AudioDto))] long id,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new SetFavoriteAudioCommand(id, userId, false);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            isFavorited => isFavorited,
            _ => throw new EntityNotFoundException<Audio, long>(id));
    }
}