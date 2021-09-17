using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using Audiochan.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Audios.RemoveAudio
{
    public record RemoveAudioCommand(long Id) : IRequest<Result<bool>>
    {
    }

    public class RemoveAudioCommandHandler : IRequestHandler<RemoveAudioCommand, Result<bool>>
    {
        private readonly IAuthService _authService;
        private readonly IStorageService _storageService;
        private readonly AudioStorageSettings _audioStorageSettings;
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveAudioCommandHandler(IAuthService authService, 
            IImageService imageService, 
            IUnitOfWork unitOfWork, 
            IStorageService storageService, 
            IOptions<MediaStorageSettings> mediaStorageOptions)
        {
            _authService = authService;
            _imageService = imageService;
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _audioStorageSettings = mediaStorageOptions.Value.Audio;
        }

        public async Task<Result<bool>> Handle(RemoveAudioCommand command, CancellationToken cancellationToken)
        {
            var currentUserId = _authService.GetUserId();

            var audio = await _unitOfWork.Audios.FindAsync(command.Id, cancellationToken);

            if (audio == null)
                return Result<bool>.NotFound<Audio>();

            if (audio.UserId != currentUserId) 
                return Result<bool>.Forbidden();
            
            var afterDeletionTasks = GetTasksForAfterDeletion(audio, cancellationToken);
            _unitOfWork.Audios.Remove(audio);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await Task.WhenAll(afterDeletionTasks);
            return Result<bool>.Success(true);
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