using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.CreateAudio
{
    public class CreateAudioCommand : IRequest<Result<Guid>>
    {
        public string UploadId { get; init; } = null!;
        public string FileName { get; init; } = null!;
        public long FileSize { get; init; }
        public decimal Duration { get; init; }
        public string Title { get; init; } = null!;
        public string Description { get; init; } = string.Empty;
        public Visibility Visibility { get; init; }
        public List<string> Tags { get; init; } = new();
        public string BlobName => UploadId + Path.GetExtension(FileName);
    }

    public class CreateAudioCommandHandler : IRequestHandler<CreateAudioCommand, Result<Guid>>
    {
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly MediaStorageSettings _storageSettings;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAudioCommandHandler(IOptions<MediaStorageSettings> mediaStorageOptions,
            IStorageService storageService,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _storageSettings = mediaStorageOptions.Value;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateAudioCommand command,
            CancellationToken cancellationToken)
        {
            if (!_currentUserService.TryGetUserId(out var currentUserId))
            {
                return Result<Guid>.Unauthorized();
            }

            if (!await ExistsInTempStorageAsync(command.BlobName, cancellationToken))
            {
                return Result<Guid>.BadRequest("Cannot find upload. Please upload and try again.");
            }

            Guid audioId;
            _unitOfWork.BeginTransaction();
            try
            {
                var audio = await CreateNewAudioBasedOnCommandAsync(command, currentUserId, cancellationToken);
                await _unitOfWork.Audios.AddAsync(audio, cancellationToken);
                await MoveTempAudioToPublicAsync(audio, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                audioId = audio.Id;
            }
            catch
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }

            await _unitOfWork.CommitTransactionAsync();
            
            return Result<Guid>.Success(audioId);
        }

        private async Task<Audio> CreateNewAudioBasedOnCommandAsync(CreateAudioCommand command, string ownerId,
            CancellationToken cancellationToken = default)
        {
            var audio = new Audio
            {
                UserId = ownerId,
                Size = command.FileSize,
                Duration = command.Duration,
                Title = command.Title,
                Description = command.Description,
                Visibility = command.Visibility,
                File = command.BlobName,
            };

            if (command.Tags.Count > 0)
            {
                audio.Tags = await _unitOfWork.Tags.GetAppropriateTags(command.Tags, cancellationToken);
            }

            return audio;
        }

        private async Task MoveTempAudioToPublicAsync(Audio audio, CancellationToken cancellationToken)
        {
            // Copy temp audio to public bucket
            await _storageService.MoveBlobAsync(
                _storageSettings.Audio.TempBucket,
                _storageSettings.Audio.Container,
                audio.File,
                _storageSettings.Audio.Bucket,
                _storageSettings.Audio.Container,
                audio.File,
                cancellationToken);
        }

        private async Task<bool> ExistsInTempStorageAsync(string blobName, CancellationToken cancellationToken = default)
        {
            return await _storageService.ExistsAsync(
                _storageSettings.Audio.TempBucket,
                _storageSettings.Audio.Container,
                blobName,
                cancellationToken);
        }
    }
}