using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Audios.Errors;
using Audiochan.API.GraphQL.Errors;
using Audiochan.Shared.Extensions;
using Audiochan.Core.Entities;
using Audiochan.Core.Exceptions;
using Audiochan.Core.Features.Audios.Commands;
using Audiochan.Core.Features.Audios.Models;
using HotChocolate.Authorization;
using HotChocolate.Language;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using MediatR;

namespace Audiochan.API.Features.Audios;

[ExtendObjectType(OperationType.Mutation)]
public class AudioMutations
{
    [Authorize]
    [UseValidationError]
    public async Task<CreateAudioResult> CreateAudioAsync(
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
    public async Task<AudioDto> PublishAudioAsync(
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
    public async Task<AudioDto> UpdateAudioAsync(
        [ID(nameof(AudioDto))] long id,
        string? title,
        string? description,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAudioCommand(id, title, description, claimsPrincipal);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            audio => audio,
            _ => throw new EntityNotFoundException<Audio, long>(id),
            _ => throw new EntityNotFoundException<Audio, long>(id));
    }
    
    [Authorize]
    [UseMutationConvention(PayloadFieldName = "url")]
    [UseValidationError]
    [Error(typeof(AudioNotFoundError))]
    public async Task<string> UpdateAudioPictureAsync(
        [ID(nameof(AudioDto))] long id,
        string data,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAudioPictureCommand(id, data, claimsPrincipal);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            res => res,
            _ => throw new EntityNotFoundException<Audio, long>(id),
            _ => throw new EntityNotFoundException<Audio, long>(id));
    }

    [Authorize]
    [UseMutationConvention(PayloadFieldName = "removed")]
    [Error(typeof(AudioNotFoundError))]
    public async Task<bool> RemoveAudioAsync(
        [ID(nameof(AudioDto))] long id,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new RemoveAudioCommand(id, claimsPrincipal);
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
    public async Task<bool> RemoveAudioPictureAsync(
        [ID(nameof(AudioDto))] long id,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAudioPictureCommand(id, null, claimsPrincipal);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            string.IsNullOrEmpty,
            _ => throw new EntityNotFoundException<Audio, long>(id),
            _ => throw new EntityNotFoundException<Audio, long>(id));
    }

    [Authorize]
    [UseMutationConvention(PayloadFieldName = "favorited")]
    [Error(typeof(AudioNotFoundError))]
    public async Task<bool> FavoriteAudioAsync(
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
    public async Task<bool> UnfavoriteAudioAsync(
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