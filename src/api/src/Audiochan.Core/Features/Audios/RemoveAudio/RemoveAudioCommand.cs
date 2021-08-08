using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.RemoveAudio
{
    public record RemoveAudioCommand(Guid Id) : IRequest<Result<bool>>
    {
    }

    public class RemoveAudioCommandHandler : IRequestHandler<RemoveAudioCommand, Result<bool>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly MediaStorageSettings _storageSettings;
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;

        public RemoveAudioCommandHandler(IOptions<MediaStorageSettings> options,
            IStorageService storageService,
            ICurrentUserService currentUserService, 
            ApplicationDbContext dbContext)
        {
            _storageSettings = options.Value;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _dbContext = dbContext;
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
                _storageService.RemoveAsync(
                    _storageSettings.Audio.Bucket,
                    _storageSettings.Audio.Container,
                    $"{audio.Id}/{audio.File}",
                    cancellationToken)
            };

            if (!string.IsNullOrEmpty(audio.Picture))
            {
                tasks.Add(_storageService.RemoveAsync(_storageSettings.Image.Bucket,
                    string.Join('/', _storageSettings.Image.Container, "audios"),
                    audio.Picture,
                    cancellationToken));
            }

            return tasks;
        }
    }
}