using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Errors;
using Audiochan.API.Features.Audios.Errors;
using Audiochan.Common.Extensions;
using Audiochan.Core.Features.Audios.Commands;
using Audiochan.Core.Features.Audios.DataLoaders;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Upload.Commands;
using Audiochan.Core.Features.Upload.Models;
using HotChocolate.Authorization;
using HotChocolate.Language;
using HotChocolate.Types;
using MediatR;

namespace Audiochan.API.Features.Audios;

[ExtendObjectType(OperationType.Mutation)]
public class AudioMutations
{
    [Authorize]
    [Error<ValidationError>]
    public async Task<CreateUploadResult> CreateUploadLinkAsync(
        string fileName,
        long fileSize,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new CreateAudioUploadCommand(fileName, fileSize, userId);
        return await mediator.Send(command, cancellationToken);
    }

    [Authorize]
    [Error<AudioNotUploadedError>]
    [Error<ValidationError>]
    public async Task<AudioViewModel?> CreateAudioAsync(
        string uploadId,
        string fileName,
        long fileSize,
        decimal duration,
        string title,
        string description,
        IMediator mediator,
        GetAudioDataLoader audioDataLoader,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new CreateAudioCommand(uploadId, fileName, fileSize, duration, title, description, claimsPrincipal);
        var result = await mediator.Send(command, cancellationToken);
        var audio = await audioDataLoader.LoadAsync(result, cancellationToken);
        return audio;
    }

    [Authorize]
    [Error<AudioNotFoundError>]
    [Error<ValidationError>]
    public async Task<AudioViewModel> UpdateAudioAsync(
        long id,
        string? title,
        string? description,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAudioCommand(id, title, description, claimsPrincipal);
        return await mediator.Send(command, cancellationToken);
    }
    
    [Authorize]
    [Error<AudioNotFoundError>]
    [Error<ValidationError>]
    public async Task<ImageUploadResult> UpdateAudioPictureAsync(
        long id,
        string data,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAudioPictureCommand(id, data, claimsPrincipal);
        return await mediator.Send(command, cancellationToken);
    }

    [Authorize]
    [Error<AudioNotFoundError>]
    public async Task<bool> RemoveAudioAsync(
        long id,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new RemoveAudioCommand(id, claimsPrincipal);
        return await mediator.Send(command, cancellationToken);
    }

    [Authorize]
    [Error<AudioNotFoundError>]
    [Error<ValidationError>]
    public async Task<ImageUploadResult> RemoveAudioPictureAsync(
        long id,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAudioPictureCommand(id, null, claimsPrincipal);
        return await mediator.Send(command, cancellationToken);
    }

    [Authorize]
    [Error<AudioNotFoundError>]
    public async Task<bool> FavoriteAudioAsync(long audioId, 
        IMediator mediator, 
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new SetFavoriteAudioCommand(audioId, userId, true);
        return await mediator.Send(command, cancellationToken);
    }
    
    [Authorize]
    [Error<AudioNotFoundError>]
    public async Task<bool> UnfavoriteAudioAsync(long audioId, 
        IMediator mediator, 
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new SetFavoriteAudioCommand(audioId, userId, false);
        return await mediator.Send(command, cancellationToken);
    }
}