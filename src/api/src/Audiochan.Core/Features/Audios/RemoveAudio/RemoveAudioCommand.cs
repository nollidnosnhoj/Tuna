using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.RemoveAudio
{
    public record RemoveAudioCommand(long Id) : IRequest<Result<bool>>
    {
    }

    public class RemoveAudioCommandHandler : IRequestHandler<RemoveAudioCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IStorageService _storageService;
        private readonly AudioStorageSettings _audioStorageSettings;
        private readonly IImageUploadService _imageUploadService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveAudioCommandHandler(ICurrentUserService currentUserService, 
            IImageUploadService imageUploadService, 
            IUnitOfWork unitOfWork, 
            IStorageService storageService, 
            IOptions<MediaStorageSettings> mediaStorageOptions)
        {
            _currentUserService = currentUserService;
            _imageUploadService = imageUploadService;
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _audioStorageSettings = mediaStorageOptions.Value.Audio;
        }

        public async Task<Result<bool>> Handle(RemoveAudioCommand command, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

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
                tasks.Add(_imageUploadService.RemoveImage(AssetContainerConstants.AudioPictures, 
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