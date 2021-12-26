using Audiochan.Application.Commons.Dtos.Responses;
using Audiochan.Application.Features.Audios.Commands.CreateAudio;
using Audiochan.Application.Features.Audios.Commands.RemoveAudio;
using Audiochan.Application.Features.Audios.Commands.RemovePicture;
using Audiochan.Application.Features.Audios.Commands.UpdateAudio;
using Audiochan.Application.Features.Audios.Commands.UpdatePicture;
using Audiochan.Application.Features.Upload.Commands.CreateUpload;
using Audiochan.Domain.Entities;
using Audiochan.GraphQL.Audios.Errors;
using Audiochan.GraphQL.Common.Errors;
using HotChocolate.AspNetCore.Authorization;
using MediatR;
// ReSharper disable UnusedMember.Global

namespace Audiochan.GraphQL.Audios;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class AudioMutations
{
    [UseMutationConvention]
    [Authorize]
    [Error(typeof(Unauthorized))]
    [Error(typeof(UploadDoesNotExist))]
    public async Task<Audio?> CreateAudio(
        string uploadId,
        string title,
        string description,
        List<string> tags,
        string fileName,
        long fileSize,
        decimal duration,
        [Service] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateAudioCommand(uploadId, title, description, tags, fileName, fileSize, duration);
        return await mediator.Send(command, cancellationToken);
    }

    [UseMutationConvention]
    [Authorize]
    [Error(typeof(AudioNotFound))]
    [Error(typeof(Forbidden))]
    public async Task<Audio?> UpdateAudio(
        [ID(nameof(Audio))] long id,
        string? title,
        string? description,
        List<string>? tags,
        [Service] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateAudioCommand(id, title, description, tags);
        return await mediator.Send(command, cancellationToken);
    }
    
    [UseMutationConvention(PayloadFieldName = "response")]
    [Authorize]
    [Error(typeof(AudioNotFound))]
    [Error(typeof(Forbidden))]
    public async Task<ImageUploadResponse> UpdateAudioPicture(
        [ID(nameof(Audio))] long id,
        string data,
        [Service] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateAudioPictureCommand(id, data);
        return await mediator.Send(command, cancellationToken);
    }

    [UseMutationConvention(PayloadFieldName = "success")]
    [Authorize]
    [Error(typeof(AudioNotFound))]
    [Error(typeof(Forbidden))]
    public async Task<bool> RemoveAudio(
        [ID(nameof(Audio))] long id,
        [Service] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveAudioCommand(id);
        await mediator.Send(command, cancellationToken);
        return true;
    }
    
    [UseMutationConvention(PayloadFieldName = "success")]
    [Authorize]
    [Error(typeof(AudioNotFound))]
    [Error(typeof(Forbidden))]
    public async Task<bool> RemoveAudioPicture(
        [ID(nameof(Audio))] long id,
        [Service] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveAudioPictureCommand(id);
        await mediator.Send(command, cancellationToken);
        return true;
    }

    [UseMutationConvention(PayloadFieldName = "response")]
    [Authorize]
    public async Task<GenerateUploadLinkResponse?> GenerateUploadLink(
        string fileName,
        long filesize,
        [Service] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new GenerateUploadLinkCommand(fileName, filesize);
        return await mediator.Send(command, cancellationToken);
    }
}