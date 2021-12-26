using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Exceptions;
using Audiochan.Application.Commons.Services;
using Audiochan.Application.Persistence;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Application.Features.Audios.Commands.RemoveAudio
{
    public record RemoveAudioCommand(long Id) : ICommandRequest
    {
    }

    public class RemoveAudioCommandHandler : IRequestHandler<RemoveAudioCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IStorageService _storageService;
        private readonly AudioStorageSettings _audioStorageSettings;
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveAudioCommandHandler(ICurrentUserService currentUserService, 
            IImageService imageService, 
            IUnitOfWork unitOfWork, 
            IStorageService storageService, 
            IOptions<MediaStorageSettings> mediaStorageOptions)
        {
            _currentUserService = currentUserService;
            _imageService = imageService;
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _audioStorageSettings = mediaStorageOptions.Value.Audio;
        }

        public async Task<Unit> Handle(RemoveAudioCommand command, CancellationToken cancellationToken)
        {
            _currentUserService.User.TryGetUserId(out var currentUserId);

            var audio = await _unitOfWork.Audios.FindAsync(command.Id, cancellationToken);

            if (audio == null)
                throw new NotFoundException<Audio, long>(command.Id);

            if (audio.UserId != currentUserId)
                throw new ForbiddenException();
            
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
                RemoveAudioFromStorage(audio.File, cancellationToken)
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