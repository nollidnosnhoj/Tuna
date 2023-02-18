using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Common.Dtos;
using Audiochan.Common.Extensions;
using Audiochan.Common.Services;
using Audiochan.Core.Features.Audios.Exceptions;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using Audiochan.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

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
        private readonly AudioStorageSettings _audioStorageSettings;
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveAudioCommandHandler(
            IImageService imageService, 
            IUnitOfWork unitOfWork, 
            IStorageService storageService, 
            IOptions<MediaStorageSettings> mediaStorageOptions)
        {
            _imageService = imageService;
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _audioStorageSettings = mediaStorageOptions.Value.Audio;
        }

        public async Task<bool> Handle(RemoveAudioCommand command, CancellationToken cancellationToken)
        {
            var currentUserId = command.GetUserId();

            var audio = await _unitOfWork.Audios.FindAsync(command.Id, cancellationToken);

            if (audio == null)
                throw new AudioNotFoundException(command.Id);

            if (audio.UserId != currentUserId) 
                throw new AudioNotFoundException(command.Id);
            
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
                RemoveAudioFromStorage(audio.ObjectKey, cancellationToken)
            };

            if (!string.IsNullOrEmpty(audio.Picture))
            {
                tasks.Add(_imageService.RemoveImage(AssetContainerConstants.AUDIO_PICTURES, 
                    audio.Picture, 
                    cancellationToken));
            }

            return tasks;
        }
        
        private async Task RemoveAudioFromStorage(string fileName, CancellationToken cancellationToken = default)
        {
            await _storageService.RemoveAsync(
                _audioStorageSettings.Bucket,
                $"audios/{fileName}",
                cancellationToken);
        }
    }
}