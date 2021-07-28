using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Interfaces;
using FluentValidation;
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
    
    public class CreateAudioCommandValidator : AbstractValidator<CreateAudioCommand>
    {
        public CreateAudioCommandValidator(IOptions<MediaStorageSettings> options)
        {
            var storageSettings = options.Value;
            RuleFor(req => req.UploadId)
                .NotEmpty()
                .WithMessage("UploadId is required.");
            RuleFor(req => req.Duration)
                .NotEmpty()
                .WithMessage("Duration is required.");
            RuleFor(req => req.FileSize)
                .FileSizeValidation(storageSettings.Audio.MaximumFileSize);
            RuleFor(req => req.FileName)
                .FileNameValidation(storageSettings.Audio.ValidContentTypes);
            RuleFor(req => req.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(30)
                .WithMessage("Title cannot be no more than 30 characters long.");
            RuleFor(req => req.Description)
                .NotNull()
                .WithMessage("Description cannot be null.")
                .MaximumLength(500)
                .WithMessage("Description cannot be more than 500 characters long.");
            RuleFor(req => req.Tags)
                .NotNull()
                .WithMessage("Tags cannot be null.")
                .Must(u => u!.Count <= 10)
                .WithMessage("Can only have up to 10 tags per audio upload.")
                .ForEach(tagsRule =>
                {
                    tagsRule
                        .NotEmpty()
                        .WithMessage("Each tag cannot be empty.")
                        .Length(3, 15)
                        .WithMessage("Each tag must be between 3 and 15 characters long.");
                });
        }
    }


    public class CreateAudioCommandHandler : IRequestHandler<CreateAudioCommand, Result<Guid>>
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly MediaStorageSettings _storageSettings;

        public CreateAudioCommandHandler(IOptions<MediaStorageSettings> mediaStorageOptions,
            IStorageService storageService,
            ICurrentUserService currentUserService, 
            ApplicationDbContext applicationDbContext)
        {
            _storageSettings = mediaStorageOptions.Value;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _applicationDbContext = applicationDbContext;
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
            
            var audio = await CreateNewAudioBasedOnCommandAsync(command, currentUserId, cancellationToken);
            await _applicationDbContext.Audios.AddAsync(audio, cancellationToken);
            await MoveTempAudioToPublicAsync(audio, cancellationToken);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(audio.Id);
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
                audio.Tags = await _applicationDbContext.Tags.GetAppropriateTags(command.Tags, cancellationToken);
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