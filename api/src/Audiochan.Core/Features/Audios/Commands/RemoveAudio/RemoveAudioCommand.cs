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

namespace Audiochan.Core.Features.Audios.Commands.RemoveAudio
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

        public RemoveAudioCommandHandler(
            IUnitOfWork unitOfWork, 
            IStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
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
        
            if (!string.IsNullOrEmpty(audio.Picture))
            {
                tasks.Add(_storageService.RemoveAsync(
                    "audiochan",     // TODO: add bucket to configuration
                    $"{AssetContainerConstants.AUDIO_PICTURES}/{audio.Picture}", 
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
                "audiochan",    // TODO: add bucket to configuration
                $"audios/{uploadId}/{audio.ObjectKey}",
                cancellationToken);
        }
    }
}