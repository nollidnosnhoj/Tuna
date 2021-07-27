using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Services;
using FastExpressionCompiler;
using MediatR;

namespace Audiochan.Core.Features.Audios.UpdateAudio
{
    public class UpdateAudioCommand : IRequest<Result<AudioViewModel>>
    {
        public Guid AudioId { get; set; }
        public string? Title { get; init; }
        public string? Description { get; init; }
        public Visibility? Visibility { get; init; }
        public List<string>? Tags { get; init; }

        public static UpdateAudioCommand FromRequest(Guid audioId, UpdateAudioRequest request) => new()
        {
            AudioId = audioId,
            Tags = request.Tags,
            Title = request.Title,
            Visibility = request.Visibility,
            Description = request.Description,
        };
    }

    public class UpdateAudioCommandHandler : IRequestHandler<UpdateAudioCommand, Result<AudioViewModel>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public UpdateAudioCommandHandler(ICurrentUserService currentUserService, 
            IUnitOfWork unitOfWork,
            ICacheService cacheService)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<Result<AudioViewModel>> Handle(UpdateAudioCommand command,
            CancellationToken cancellationToken)
        {
            if (!_currentUserService.TryGetUserId(out var currentUserId))
                return Result<AudioViewModel>.Unauthorized();

            var audio = await _unitOfWork.Audios
                .LoadForUpdating(command.AudioId, cancellationToken);

            if (audio == null)
                return Result<AudioViewModel>.NotFound<Audio>();

            if (audio.UserId != currentUserId)
                return Result<AudioViewModel>.Forbidden();
            
            _unitOfWork.BeginTransaction();
            try
            {
                await UpdateAudioFromCommandAsync(audio, command, cancellationToken);
                _unitOfWork.Audios.Update(audio);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _cacheService.RemoveAsync(new GetAudioCacheOptions(audio.Id), cancellationToken);
            }
            catch
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }

            await _unitOfWork.CommitTransactionAsync();
            return Result<AudioViewModel>.Success(AudioMaps.AudioToView.CompileFast().Invoke(audio));
        }

        private async Task UpdateAudioFromCommandAsync(Audio audio, UpdateAudioCommand command,
            CancellationToken cancellationToken)
        {
            if (command.Tags is not null)
            {
                if (command.Tags.Count == 0)
                {
                    audio.Tags.Clear();
                }
                else
                {
                    var newTags = await _unitOfWork.Tags.GetAppropriateTags(command.Tags, cancellationToken);

                    audio.UpdateTags(newTags);
                }
            }

            if (command.Title is not null && !string.IsNullOrWhiteSpace(command.Title))
            {
                audio.Title = command.Title;
            }

            if (command.Description is not null)
            {
                audio.Description = command.Description;
            }

            if (command.Visibility.HasValue)
            {
                audio.Visibility = command.Visibility.Value;
            }
        }
    }
}