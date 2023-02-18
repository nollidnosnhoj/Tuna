using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Dtos;
using Audiochan.Common.Extensions;
using Audiochan.Core.Features.Audios.Commands.CreateAudio;
using Audiochan.Core.Features.Audios.Commands.RemoveAudio;
using Audiochan.Core.Features.Audios.Commands.UpdateAudio;
using Audiochan.Core.Features.Audios.Commands.UpdatePicture;
using Audiochan.Core.Features.Audios.DataLoaders;
using Audiochan.Core.Features.Audios.Dtos;
using Audiochan.Core.Features.Audios.Exceptions;
using Audiochan.Core.Features.Upload.Commands.CreateUpload;
using Audiochan.Core.Features.Users.Commands.SetFavoriteAudio;
using FluentValidation;
using HotChocolate.Authorization;
using HotChocolate.Language;
using HotChocolate.Types;
using MediatR;

namespace Audiochan.API.Mutations;

[ExtendObjectType(OperationType.Mutation)]
public class AudioMutations
{
    [Authorize]
    [Error<ValidationException>]
    public async Task<GenerateUploadLinkResponse> CreateUploadLinkAsync(
        string fileName,
        long fileSize,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new GenerateUploadLinkCommand(fileName, fileSize, claimsPrincipal);
        return await mediator.Send(command, cancellationToken);
    }

    [Authorize]
    [Error<AudioNotUploadedException>]
    [Error<ValidationException>]
    public async Task<AudioDto?> CreateAudioAsync(
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
    [Error<AudioNotFoundException>]
    [Error<ValidationException>]
    public async Task<AudioDto> UpdateAudioAsync(
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
    [Error<AudioNotFoundException>]
    [Error<ValidationException>]
    public async Task<ImageUploadResponse> UpdateAudioPictureAsync(
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
    [Error<AudioNotFoundException>]
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
    [Error<AudioNotFoundException>]
    [Error<ValidationException>]
    public async Task<ImageUploadResponse> RemoveAudioPictureAsync(
        long id,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAudioPictureCommand(id, null, claimsPrincipal);
        return await mediator.Send(command, cancellationToken);
    }

    [Authorize]
    [Error<AudioNotFoundException>]
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
    [Error<AudioNotFoundException>]
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