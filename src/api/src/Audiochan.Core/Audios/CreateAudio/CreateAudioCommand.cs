using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Attributes;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using Audiochan.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Audios
{
    [Authorize]
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
            _currentUserService.User.TryGetUserId(out var currentUserId);

            if (!await ExistsInTempStorage(command.BlobName, cancellationToken))
            {
                return Result<long>.BadRequest("Cannot find upload. Please upload and try again.");
            }
            
            var audio = new Audio
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
                audio.Tags = _slugGenerator.GenerateSlugs(command.Tags).ToList();
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