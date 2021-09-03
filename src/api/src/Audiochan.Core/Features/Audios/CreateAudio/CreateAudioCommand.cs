using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.CreateAudio
{
    public class CreateAudioCommand : IRequest<Result<long>>
    {
        public string UploadId { get; init; } = null!;
        public string FileName { get; init; } = null!;
        public long FileSize { get; init; }
        public decimal Duration { get; init; }
        public string Title { get; init; } = null!;
        public string Description { get; init; } = string.Empty;
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


    public class CreateAudioCommandHandler : IRequestHandler<CreateAudioCommand, Result<long>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ISlugGenerator _slugGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        private readonly AudioStorageSettings _audioStorageSettings;

        public CreateAudioCommandHandler(ICurrentUserService currentUserService, 
            ISlugGenerator slugGenerator,
            IUnitOfWork unitOfWork, 
            IStorageService storageService,
            IOptions<MediaStorageSettings> mediaStorageOptions)
        {
            _currentUserService = currentUserService;
            _slugGenerator = slugGenerator;
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _audioStorageSettings = mediaStorageOptions.Value.Audio;
        }

        public async Task<Result<long>> Handle(CreateAudioCommand command,
            CancellationToken cancellationToken)
        {
            if (!_currentUserService.TryGetUserId(out var currentUserId))
            {
                return Result<long>.Unauthorized();
            }

            if (!await ExistsInTempStorage(command.BlobName, cancellationToken))
            {
                return Result<long>.BadRequest("Cannot find upload. Please upload and try again.");
            }
            
            Audio audio = new Audio
            {
                UserId = currentUserId,
                Size = command.FileSize,
                Duration = command.Duration,
                Title = command.Title,
                Description = command.Description,
                File = command.BlobName,
            };

            // Create tags
            if (command.Tags.Count > 0)
            {
                var tags = _slugGenerator.GenerateSlugs(command.Tags);
                audio.Tags = await _unitOfWork.Tags.GetAppropriateTags(tags, cancellationToken);
            }

            await _unitOfWork.Audios.AddAsync(audio, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await MoveTempAudioToPublic(audio.File, cancellationToken);
            return Result<long>.Success(audio.Id);
        }
        
        private async Task MoveTempAudioToPublic(string fileName, CancellationToken cancellationToken = default)
        {
            await _storageService.MoveBlobAsync(
                _audioStorageSettings.TempBucket,
                fileName,
                _audioStorageSettings.Bucket,
                $"audios/{fileName}",
                cancellationToken: cancellationToken);
        }
        
        private async Task<bool> ExistsInTempStorage(string fileName, CancellationToken cancellationToken = default)
        {
            return await _storageService.ExistsAsync(
                _audioStorageSettings.TempBucket,
                fileName,
                cancellationToken);
        }
    }
}