using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using Audiochan.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.RemoveAudio
{
    public record RemoveAudioCommand(long Id) : IRequest<Result<bool>>
    {
    }

    public class RemoveAudioCommandHandler : IRequestHandler<RemoveAudioCommand, Result<bool>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAudioUploadService _audioUploadService;
        private readonly IImageUploadService _imageUploadService;

        public RemoveAudioCommandHandler(ICurrentUserService currentUserService, 
            ApplicationDbContext dbContext, IAudioUploadService audioUploadService, 
            IImageUploadService imageUploadService)
        {
            _currentUserService = currentUserService;
            _dbContext = dbContext;
            _audioUploadService = audioUploadService;
            _imageUploadService = imageUploadService;
        }

        public async Task<Result<bool>> Handle(RemoveAudioCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUserService.TryGetUserId(out var currentUserId))
                return Result<bool>.Unauthorized();

            var audio = await _dbContext.Audios
                .SingleOrDefaultAsync(a => a.Id == command.Id, cancellationToken);

            if (audio == null)
                return Result<bool>.NotFound<Audio>();

            if (!ShouldCurrentUserModifyAudio(audio, currentUserId))
                return Result<bool>.Forbidden();
            
            var afterDeletionTasks = GetTasksForAfterDeletion(audio, cancellationToken);
            _dbContext.Audios.Remove(audio);
            await _dbContext.SaveChangesAsync(cancellationToken);
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