using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Audios.GetAudio;
using Audiochan.API.Features.Shared.Requests;
using Audiochan.API.Mappings;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using Audiochan.Core.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.API.Features.Audios.CreateAudio
{
    public class CreateAudioRequest : AudioAbstractRequest, IRequest<Result<AudioDetailViewModel>>
    {
        public string UploadId { get; init; } = null!;
        public string FileName { get; init; } = null!;
        public long FileSize { get; init; }
        public string ContentType { get; init; } = null!;
        public decimal Duration { get; init; }
    }

    public class CreateAudioRequestHandler : IRequestHandler<CreateAudioRequest, Result<AudioDetailViewModel>>
    {
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly MediaStorageSettings _storageSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITagRepository _tagRepository;

        public CreateAudioRequestHandler(IOptions<MediaStorageSettings> mediaStorageOptions,
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

        public async Task<Result<AudioDetailViewModel>> Handle(CreateAudioRequest request,
            CancellationToken cancellationToken)
        {
            var existsInTempStorage = await ExistsInTempStorageAsync(request.UploadId, cancellationToken);

            if (!existsInTempStorage)
                return Result<AudioDetailViewModel>.Fail(ResultError.BadRequest, "Cannot find audio in temp storage.");
            
            var currentUser = await _unitOfWork.Users
                    .SingleOrDefaultAsync(x => x.Id == _currentUserService.GetUserId(), cancellationToken);

            if (currentUser is null)
                return Result<AudioDetailViewModel>.Fail(ResultError.Unauthorized);

            var audio = new Audio
            {
                User = currentUser,
                ContentType = request.ContentType,
                FileName = request.FileName,
                FileExt = Path.GetExtension(request.UploadId),
                FileSize = request.FileSize,
                Duration = request.Duration,
                Title = request.Title,
                Description = request.Description,
                IsPublic = request.IsPublic ?? false,
                BlobName = request.UploadId,
                Tags = request.Tags.Count > 0
                    ? await _tagRepository.GetAppropriateTags(request.Tags, cancellationToken)
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