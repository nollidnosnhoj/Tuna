using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Errors;
using Audiochan.API.Features.Audios.Errors;
using Audiochan.API.Features.Audios.Inputs;
using Audiochan.API.Features.Audios.Payloads;
using Audiochan.Common.Extensions;
using Audiochan.Common.Models;
using Audiochan.Core.Features.Audios.Commands;
using HotChocolate.Authorization;
using HotChocolate.Language;
using HotChocolate.Types;
using MediatR;

namespace Audiochan.API.Features.Audios;

[ExtendObjectType(OperationType.Mutation)]
public class AudioMutations
{
    [Authorize]
    [UseValidationError]
    public async Task<AudioPayload> CreateAudioAsync(
        CreateAudioInput input,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new CreateAudioCommand(
            input.UploadId,
            input.FileName,
            input.FileSize,
            input.Duration,
            input.Title,
            input.Description,
            claimsPrincipal.GetUserId());
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            audio => new AudioPayload(audio),
            error => new AudioPayload(new AudioNotUploadedError(error.UploadId)));
    }

    [Authorize]
    [UseValidationError]
    public async Task<AudioPayload> UpdateAudioAsync(
        UpdateAudioInput input,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var (id, title, description) = input;
        var command = new UpdateAudioCommand(id, title, description, claimsPrincipal);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            audio => new AudioPayload(audio),
            _ => new AudioPayload(new AudioNotFoundError(id)),
            _ => new AudioPayload(new AudioNotFoundError(id)));
    }
    
    [Authorize]
    [UseValidationError]
    public async Task<UpdatePicturePayload> UpdateAudioPictureAsync(
        UpdateAudioPictureInput input,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var (id, data) = input;
        var command = new UpdateAudioPictureCommand(id, data, claimsPrincipal);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            res => new UpdatePicturePayload(res),
            _ => new UpdatePicturePayload(new AudioNotFoundError(id)),
            _ => new UpdatePicturePayload(new AudioNotFoundError(id)));
    }

    [Authorize]
    public async Task<RemoveAudioPayload> RemoveAudioAsync(
        RemoveAudioInput input,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new RemoveAudioCommand(input.Id, claimsPrincipal);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            _ => new RemoveAudioPayload(),
            _ => new RemoveAudioPayload(new AudioNotFoundError(input.Id)),
            _ => new RemoveAudioPayload(new AudioNotFoundError(input.Id)));
    }

    [Authorize]
    [UseValidationError]
    public async Task<RemovePicturePayload> RemoveAudioPictureAsync(
        RemoveAudioPictureInput input,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAudioPictureCommand(input.Id, null, claimsPrincipal);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            _ => new RemovePicturePayload(),
            _ => new RemovePicturePayload(new AudioNotFoundError(input.Id)),
            _ => new RemovePicturePayload(new AudioNotFoundError(input.Id)));
    }

    [Authorize]
    public async Task<SetFavoriteAudioPayload> FavoriteAudioAsync(
        SetFavoriteAudioInput input,
        IMediator mediator, 
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new SetFavoriteAudioCommand(input.AudioId, userId, true);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            isFavorited => new SetFavoriteAudioPayload(isFavorited),
            _ => new SetFavoriteAudioPayload(new AudioNotFoundError(input.AudioId)));
    }
    
    [Authorize]
    public async Task<SetFavoriteAudioPayload> UnfavoriteAudioAsync(
        SetFavoriteAudioInput input,
        IMediator mediator, 
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new SetFavoriteAudioCommand(input.AudioId, userId, false);
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            isFavorited => new SetFavoriteAudioPayload(isFavorited),
            _ => new SetFavoriteAudioPayload(new AudioNotFoundError(input.AudioId)));
    }
}