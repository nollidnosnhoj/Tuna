using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Shared.Requests;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.CreateAudio
{
    public class CreateAudioCommand : AudioAbstractRequest, IRequest<Result<AudioDetailViewModel>>
    {
        public string UploadId { get; init; } = null!;
        public string FileName { get; init; } = null!;
        public long FileSize { get; init; }
        public string ContentType { get; init; } = null!;
        public decimal Duration { get; init; }
    }

    public class CreateAudioCommandHandler : IRequestHandler<CreateAudioCommand, Result<AudioDetailViewModel>>
    {
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly MediaStorageSettings _storageSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITagRepository _tagRepository;

        public CreateAudioCommandHandler(IOptions<MediaStorageSettings> mediaStorageOptions,
            IStorageService storageService,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork, 
            ITagRepository tagRepository)
        {
            _storageSettings = mediaStorageOptions.Value;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _tagRepository = tagRepository;
        }

        public async Task<Result<AudioDetailViewModel>> Handle(CreateAudioCommand command,
            CancellationToken cancellationToken)
        {
            var existsInTempStorage = await ExistsInTempStorageAsync(command.UploadId, cancellationToken);

            if (!existsInTempStorage)
                return Result<AudioDetailViewModel>.Fail(ResultError.BadRequest, "Cannot find audio in temp storage.");
            
            var currentUser = await _unitOfWork.Users
                    .SingleOrDefaultAsync(x => x.Id == _currentUserService.GetUserId(), cancellationToken);

            if (currentUser is null)
                return Result<AudioDetailViewModel>.Fail(ResultError.Unauthorized);

            var audio = new Audio
            {
                User = currentUser,
                ContentType = command.ContentType,
                FileName = command.FileName,
                FileExt = Path.GetExtension(command.UploadId),
                FileSize = command.FileSize,
                Duration = command.Duration,
                Title = command.Title,
                Description = command.Description,
                IsPublic = command.IsPublic ?? false,
                BlobName = command.UploadId,
                Tags = command.Tags.Count > 0
                    ? await _tagRepository.GetAppropriateTags(command.Tags, cancellationToken)
                    : new List<Tag>(),
            };

            await _unitOfWork.Audios.AddAsync(audio, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Copy temp audio to public bucket
            await _storageService.MoveBlobAsync(
                _storageSettings.Audio.TempBucket,
                _storageSettings.Audio.Container,
                audio.BlobName,
                _storageSettings.Audio.Bucket,
                _storageSettings.Audio.Container,
                $"{audio.Id}/{audio.BlobName}",
                cancellationToken);

            return Result<AudioDetailViewModel>.Success(audio.MapToDetail());
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