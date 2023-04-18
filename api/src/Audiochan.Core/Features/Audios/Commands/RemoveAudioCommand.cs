using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Shared.Errors;
using Audiochan.Shared.Mediatr;
using Audiochan.Core.Entities;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using Audiochan.Shared;
using MediatR;
using Microsoft.Extensions.Options;
using OneOf;
using OneOf.Types;

namespace Audiochan.Core.Features.Audios.Commands;

public class RemoveAudioCommand : AuthCommandRequest<RemoveAudioResult>
{
    public RemoveAudioCommand(long id, ClaimsPrincipal user) : base(user)
    {
        Id = id;
    }
        
    public long Id { get; }
}

[GenerateOneOf]
public partial class RemoveAudioResult : OneOfBase<Unit, NotFound, Forbidden>
{
    
}

public class RemoveAudioCommandHandler : IRequestHandler<RemoveAudioCommand, RemoveAudioResult>
{
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationSettings _appSettings;

    public RemoveAudioCommandHandler(
        IUnitOfWork unitOfWork, 
        IStorageService storageService, 
        IOptions<ApplicationSettings> appSettings)
    {
        _unitOfWork = unitOfWork;
        _storageService = storageService;
        _appSettings = appSettings.Value;
    }

    public async Task<RemoveAudioResult> Handle(RemoveAudioCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = command.GetUserId();

        var audio = await _unitOfWork.Audios.FindAsync(command.Id, cancellationToken);

        if (audio == null) return new NotFound();

        if (audio.UserId != currentUserId) return new Forbidden();
            
        // TODO: Make this a job
        var afterDeletionTasks = GetTasksForAfterDeletion(audio, cancellationToken);
        _unitOfWork.Audios.Remove(audio);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await Task.WhenAll(afterDeletionTasks);
        return Unit.Value;
    }

    private IEnumerable<Task> GetTasksForAfterDeletion(Audio audio, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>
        {
            RemoveAudioFromStorage(audio, cancellationToken)
        };
        
        if (!string.IsNullOrEmpty(audio.ImageId))
        {
            tasks.Add(_storageService.RemoveAsync(
                bucket: _appSettings.UploadBucket,
                blobName: $"{AssetContainerConstants.AUDIO_PICTURES}/{audio.ImageId}", 
                cancellationToken));
        }
        
        return tasks;
    }
        
    private async Task RemoveAudioFromStorage(Audio audio, CancellationToken cancellationToken = default)
    {
        var uploadId = audio.FileId.Split('.').FirstOrDefault();
        if (uploadId is null)
        {
            throw new ArgumentException("Audio object key needs to have a file extension");
        }
        await _storageService.RemoveAsync(
            bucket: _appSettings.UploadBucket,
            blobName: $"audios/{uploadId}/{audio.FileId}",
            cancellationToken);
    }
}