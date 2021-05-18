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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.CreateAudio
{
    public class CreateAudioRequest : AudioAbstractRequest, IRequest<Result<AudioDetailViewModel>>
    {
        public string UploadId { get; init; }
        public string FileName { get; init; }
        public long FileSize { get; init; }
        public string ContentType { get; init; }
        public decimal Duration { get; init; }
    }

    public class CreateAudioRequestHandler : IRequestHandler<CreateAudioRequest, Result<AudioDetailViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITagRepository _tagRepository;
        private readonly MediaStorageSettings _storageSettings;

        public CreateAudioRequestHandler(IOptions<MediaStorageSettings> mediaStorageOptions,
            IApplicationDbContext dbContext,
            IStorageService storageService,
            ICurrentUserService currentUserService,
            ITagRepository tagRepository, 
            IDateTimeProvider dateTimeProvider)
        {
            _storageSettings = mediaStorageOptions.Value;
            _dbContext = dbContext;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _tagRepository = tagRepository;
        }

        public async Task<Result<AudioDetailViewModel>> Handle(CreateAudioRequest request,
            CancellationToken cancellationToken)
        {
            var existsInTempStorage = await ExistsInTempStorageAsync(request.UploadId, cancellationToken);

            if (!existsInTempStorage)
                return Result<AudioDetailViewModel>.Fail(ResultError.BadRequest, "Cannot find audio in temp storage.");

            var audio = await CreateAudioFromRequestAsync(request, cancellationToken);

            await _dbContext.Audios.AddAsync(audio, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var viewModel = audio.MapToDetail(_storageSettings);

            // Copy temp audio to public bucket
            await _storageService.MoveBlobAsync(
                _storageSettings.Audio.TempBucket, 
                _storageSettings.Audio.Container, 
                audio.FileName, 
                _storageSettings.Audio.Bucket, 
                _storageSettings.Audio.Container,
                $"{audio.Id}/{audio.FileName}",
                cancellationToken);
            
            return Result<AudioDetailViewModel>.Success(viewModel);
        }

        private async Task<Audio> CreateAudioFromRequestAsync(CreateAudioRequest request, CancellationToken cancellationToken)
        {
            var currentUser = await _dbContext.Users
                .SingleOrDefaultAsync(x => x.Id == _currentUserService.GetUserId(), cancellationToken);

            var audio = new Audio
            {
                User = currentUser,
                ContentType = request.ContentType,
                OriginalFileName = request.FileName,
                FileExt = Path.GetExtension(request.UploadId),
                FileSize = request.FileSize,
                Duration = request.Duration,
                Title = request.Title,
                Description = request.Description,
                IsPublic = request.IsPublic ?? false,
            };
            audio.FileName = request.UploadId + audio.FileExt;
            audio.Tags = request.Tags.Count > 0
                ? await _tagRepository.GetListAsync(request.Tags, cancellationToken)
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