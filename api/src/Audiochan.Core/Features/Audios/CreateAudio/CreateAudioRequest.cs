using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.CreateAudio
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

        public CreateAudioRequestHandler(IOptions<MediaStorageSettings> mediaStorageOptions,
            IStorageService storageService,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _storageSettings = mediaStorageOptions.Value;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AudioDetailViewModel>> Handle(CreateAudioRequest request,
            CancellationToken cancellationToken)
        {
            var existsInTempStorage = await ExistsInTempStorageAsync(request.UploadId, cancellationToken);

            if (!existsInTempStorage)
                return Result<AudioDetailViewModel>.Fail(ResultError.BadRequest, "Cannot find audio in temp storage.");
            
            var currentUser =
                await _unitOfWork.Users.GetByIdAsync(_currentUserService.GetUserId(), cancellationToken);

            if (currentUser is null)
                return Result<AudioDetailViewModel>.Fail(ResultError.Unauthorized);

            var audio = await CreateAudioFromRequestAsync(request, currentUser, cancellationToken);

            await _unitOfWork.Audios.AddAsync(audio, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Copy temp audio to public bucket
            await _storageService.MoveBlobAsync(
                _storageSettings.Audio.TempBucket,
                _storageSettings.Audio.Container,
                audio.FileName,
                _storageSettings.Audio.Bucket,
                _storageSettings.Audio.Container,
                $"{audio.Id}/{audio.FileName}",
                cancellationToken);

            return Result<AudioDetailViewModel>.Success(audio.MapToDetail());
        }

        private async Task<Audio> CreateAudioFromRequestAsync(CreateAudioRequest request, User user, CancellationToken cancellationToken)
        {
            var audio = new Audio
            {
                User = user,
                ContentType = request.ContentType,
                OriginalFileName = request.FileName,
                FileExt = Path.GetExtension(request.UploadId),
                FileSize = request.FileSize,
                Duration = request.Duration,
                Title = request.Title,
                Description = request.Description,
                IsPublic = request.IsPublic ?? false,
            };
            audio.FileName = request.UploadId;
            audio.Tags = request.Tags.Count > 0
                ? await _unitOfWork.Tags.GetAppropriateTags(request.Tags, cancellationToken)
                : new List<Tag>();
            return audio;
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