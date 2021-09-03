using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Core.Services;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Audios.RemoveAudio
{
    public record RemoveAudioCommand(long Id) : IRequest<Result<bool>>
    {
    }

    public class RemoveAudioCommandHandler : IRequestHandler<RemoveAudioCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAudioUploadService _audioUploadService;
        private readonly IImageUploadService _imageUploadService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveAudioCommandHandler(ICurrentUserService currentUserService, 
            IAudioUploadService audioUploadService, 
            IImageUploadService imageUploadService, 
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _audioUploadService = audioUploadService;
            _imageUploadService = imageUploadService;
            _unitOfWork = unitOfWork;
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

        private bool ShouldCurrentUserModifyAudio(Audio audio, long userId)
        {
            return audio.UserId == userId;
        }

        private IEnumerable<Task> GetTasksForAfterDeletion(Audio audio, CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task>
            {
                _audioUploadService.RemoveAudioFromStorage(audio.File, cancellationToken)
            };

            if (!string.IsNullOrEmpty(audio.Picture))
            {
                tasks.Add(_imageUploadService.RemoveImage(AssetContainerConstants.AudioPictures, 
                    audio.Picture, 
                    cancellationToken));
            }

            return tasks;
        }
    }
}