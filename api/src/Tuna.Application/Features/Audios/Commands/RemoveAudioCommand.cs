using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Options;
using Tuna.Application.Features.Audios.Exceptions;
using Tuna.Application.Persistence;
using Tuna.Application.Services;
using Tuna.Domain.Entities;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Audios.Commands;

public class RemoveAudioCommand : AuthCommandRequest<Result<bool>>
{
    public RemoveAudioCommand(long id)
    {
        Id = id;
    }

    public long Id { get; }
}

public class RemoveAudioCommandHandler : IRequestHandler<RemoveAudioCommand, Result<bool>>
{
    private readonly ApplicationSettings _appSettings;
    private readonly IImageService _imageService;
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveAudioCommandHandler(
        IUnitOfWork unitOfWork,
        IStorageService storageService,
        IOptions<ApplicationSettings> appSettings, IImageService imageService)
    {
        _unitOfWork = unitOfWork;
        _storageService = storageService;
        _imageService = imageService;
        _appSettings = appSettings.Value;
    }

    public async Task<Result<bool>> Handle(RemoveAudioCommand command, CancellationToken cancellationToken)
    {
        var audio = await _unitOfWork.Audios.FindAsync(command.Id, cancellationToken);

        if (audio == null)
            return new Result<bool>(new AudioNotFoundException(command.Id));

        if (audio.UserId != command.UserId)
            return new Result<bool>(new UnauthorizedAccessException());

        // TODO: Make this a job
        var afterDeletionTasks = GetTasksForAfterDeletion(audio, cancellationToken);
        _unitOfWork.Audios.Remove(audio);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await Task.WhenAll(afterDeletionTasks);
        return true;
    }

    private IEnumerable<Task> GetTasksForAfterDeletion(Audio audio, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>
        {
            RemoveAudioFromStorage(audio, cancellationToken)
        };

        if (!string.IsNullOrEmpty(audio.ImageId)) tasks.Add(_imageService.DeleteImageAsync(audio.ImageId));

        return tasks;
    }

    private async Task RemoveAudioFromStorage(Audio audio, CancellationToken cancellationToken = default)
    {
        var uploadId = audio.FileId.Split('.').FirstOrDefault();
        if (uploadId is null) throw new ArgumentException("Audio object key needs to have a file extension");
        await _storageService.RemoveAsync(
            _appSettings.UploadBucket,
            $"audios/{uploadId}/{audio.FileId}",
            cancellationToken);
    }
}