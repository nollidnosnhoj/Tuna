using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Services;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.CreateAudio
{
    public class CreateAudioCommand : IRequest<Result<AudioDetailViewModel>>
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

    public class CreateAudioCommandHandler : IRequestHandler<CreateAudioCommand, Result<AudioDetailViewModel>>
    {
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICacheService _cacheService;
        private readonly MediaStorageSettings _storageSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateAudioCommandHandler(IOptions<MediaStorageSettings> mediaStorageOptions,
            IStorageService storageService,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            IMapper mapper)
        {
            _storageSettings = mediaStorageOptions.Value;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<Result<AudioDetailViewModel>> Handle(CreateAudioCommand command,
            CancellationToken cancellationToken)
        {
            var currentUser = await _unitOfWork.Users
                    .LoadAsync(x => x.Id == _currentUserService.GetUserId(), 
                        cancellationToken: cancellationToken);

            if (currentUser is null)
                return Result<AudioDetailViewModel>.Fail(ResultError.Unauthorized);

            var audio = new Audio
            {
                User = currentUser,
                FileName = command.FileName,
                FileSize = command.FileSize,
                Duration = command.Duration,
                Title = command.Title,
                Description = command.Description,
                Visibility = command.Visibility,
                BlobName = command.BlobName,
                Tags = command.Tags.Count > 0
                    ? await _unitOfWork.Tags.GetAppropriateTags(command.Tags, cancellationToken)
                    : new List<Tag>(),
            };
            
            await _unitOfWork.Audios.AddAsync(audio, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await MoveTempAudioToPublicAsync(audio, cancellationToken);
            var result = _mapper.Map<AudioDetailViewModel>(audio);
            await CreateCache(result, cancellationToken);
            return Result<AudioDetailViewModel>.Success(result);
        }

        private async Task CreateCache(AudioDetailViewModel value, CancellationToken cancellationToken = default)
        {
            await _cacheService.SetAsync(value, new GetAudioCacheOptions(value.Id), cancellationToken);
        }

        private async Task MoveTempAudioToPublicAsync(Audio audio, CancellationToken cancellationToken)
        {
            // Copy temp audio to public bucket
            await _storageService.MoveBlobAsync(
                _storageSettings.Audio.TempBucket,
                _storageSettings.Audio.Container,
                audio.BlobName,
                _storageSettings.Audio.Bucket,
                _storageSettings.Audio.Container,
                $"{audio.Id}/{audio.BlobName}",
                cancellationToken);
        }

        
    }
}