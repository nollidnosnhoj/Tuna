using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Features.Audios.Exceptions;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using Audiochan.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.Commands
{
    public class RemoveAudioCommand : AuthCommandRequest<bool>
    {
        public RemoveAudioCommand(long id, ClaimsPrincipal user) : base(user)
        {
            Id = id;
        }
        
        public long Id { get; }
    }

    public class RemoveAudioCommandHandler : IRequestHandler<RemoveAudioCommand, bool>
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

        public async Task<bool> Handle(RemoveAudioCommand command, CancellationToken cancellationToken)
        {
            var currentUserId = command.GetUserId();

            var audio = await _unitOfWork.Audios.FindAsync(command.Id, cancellationToken);

            if (audio == null)
                throw new AudioNotFoundException(command.Id);

            if (audio.UserId != currentUserId) 
                throw new AudioNotFoundException(command.Id);
            
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
            var uploadId = audio.ObjectKey.Split('.').FirstOrDefault();
            if (uploadId is null)
            {
                throw new ArgumentException("Audio object key needs to have a file extension");
            }
            await _storageService.RemoveAsync(
                bucket: _appSettings.UploadBucket,
                blobName: $"audios/{uploadId}/{audio.ObjectKey}",
                cancellationToken);
        }
    }
}