using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using MediatR;
using Tuna.Application.Features.Audios.Exceptions;
using Tuna.Application.Persistence;
using Tuna.Application.Services;
using Tuna.Domain.Entities;
using Tuna.Domain.Exceptions;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Audios.Commands;

public class UpdateAudioPictureCommand : AuthCommandRequest<Result<string?>>
{
    public UpdateAudioPictureCommand(long audioId, string? uploadId)
    {
        AudioId = audioId;
        UploadId = uploadId;
    }

    public long AudioId { get; }
    public string? UploadId { get; }
}

public class UpdateAudioPictureCommandHandler : IRequestHandler<UpdateAudioPictureCommand, Result<string?>>
{
    private readonly IImageService _imageService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAudioPictureCommandHandler(IUnitOfWork unitOfWork, IImageService imageService)
    {
        _unitOfWork = unitOfWork;
        _imageService = imageService;
    }

    public async Task<Result<string?>> Handle(UpdateAudioPictureCommand command,
        CancellationToken cancellationToken)
    {
        var audio = await _unitOfWork.Audios.FindAsync(command.AudioId, cancellationToken);

        if (audio == null)
            return new Result<string?>(new AudioNotFoundException(command.AudioId));
        if (audio.UserId != command.UserId)
            return new Result<string?>(new UnauthorizedAccessException());

        if (string.IsNullOrEmpty(command.UploadId) && !string.IsNullOrEmpty(audio.ImageId))
        {
            await _imageService.DeleteImageAsync(audio.ImageId);
            audio.ImageId = null;
        }
        else
        {
            audio.ImageId = command.UploadId;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return audio.ImageId;
    }
}