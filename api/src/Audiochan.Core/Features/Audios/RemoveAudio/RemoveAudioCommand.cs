using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.RemoveAudio
{
    public record RemoveAudioCommand(Guid Id) : IRequest<Result<bool>>
    {
    }

    public class RemoveAudioCommandHandler : IRequestHandler<RemoveAudioCommand, Result<bool>>
    {
        private readonly MediaStorageSettings _storageSettings;
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveAudioCommandHandler(IOptions<MediaStorageSettings> options,
            IStorageService storageService,
            ICurrentUserService currentUserService, 
            IUnitOfWork unitOfWork)
        {
            _storageSettings = options.Value;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(RemoveAudioCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUserService.TryGetUserId(out var currentUserId))
                return Result<bool>.Fail(ResultError.Unauthorized);
        
            var audio = await _unitOfWork.Audios
                .LoadAsync(a => a.Id == command.Id, cancellationToken: cancellationToken);
            
            if (audio == null)
                return Result<bool>.Fail(ResultError.NotFound);

            if (!ShouldCurrentUserModifyAudio(audio, currentUserId))
                return Result<bool>.Fail(ResultError.Forbidden);
            
            _unitOfWork.BeginTransaction();
            
            try
            {
                var afterDeletionTasks = GetTasksForAfterDeletion(audio, cancellationToken);
                _unitOfWork.Audios.Remove(audio);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await Task.WhenAll(afterDeletionTasks);
            }
            catch
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }

            await _unitOfWork.CommitTransactionAsync();
            return Result<bool>.Success(true);
        }

        private bool ShouldCurrentUserModifyAudio(Audio audio, string userId)
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