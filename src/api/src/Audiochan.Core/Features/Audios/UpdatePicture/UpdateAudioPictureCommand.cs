using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.UpdatePicture
{
    public record UpdateAudioPictureCommand : IImageData, IRequest<Result<ImageUploadResponse>>
    {
        public long AudioId { get; init; }
        public string Data { get; init; } = string.Empty;
    }

    public class UpdateAudioCommandHandler : IRequestHandler<UpdateAudioPictureCommand, Result<ImageUploadResponse>>
    {
        private readonly MediaStorageSettings _storageSettings;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageUploadService _imageUploadService;
        private readonly ICacheService _cacheService;
        private readonly ApplicationDbContext _dbContext;

        public UpdateAudioCommandHandler(IOptions<MediaStorageSettings> options,
            IStorageService storageService,
            ICurrentUserService currentUserService,
            IImageUploadService imageUploadService,
            IDateTimeProvider dateTimeProvider, 
            ICacheService cacheService, 
            ApplicationDbContext dbContext)
        {
            _storageSettings = options.Value;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _imageUploadService = imageUploadService;
            _dateTimeProvider = dateTimeProvider;
            _cacheService = cacheService;
            _dbContext = dbContext;
        }

        public async Task<Result<ImageUploadResponse>> Handle(UpdateAudioPictureCommand command,
            CancellationToken cancellationToken)
        {
            var container = string.Join('/', _storageSettings.Image.Container, "audios");

            if (!_currentUserService.TryGetUserId(out var currentUserId))
                return Result<ImageUploadResponse>.Unauthorized();

            var audio = await _dbContext.Audios
                .Include(a => a.User)
                .Include(a => a.Tags)
                .SingleOrDefaultAsync(a => a.Id == command.AudioId, cancellationToken);

            if (audio == null)
                return Result<ImageUploadResponse>.NotFound<Audio>();

            if (audio.UserId != currentUserId)
                return Result<ImageUploadResponse>.Forbidden();
            
            var blobName = $"{audio.Id}/{_dateTimeProvider.Now:yyyyMMddHHmmss}.jpg";

            await _imageUploadService.UploadImage(command.Data, container, blobName, cancellationToken);

            if (!string.IsNullOrEmpty(audio.Picture))
                await _storageService.RemoveAsync(_storageSettings.Image.Bucket, container, audio.Picture,
                    cancellationToken);

            audio.Picture = blobName;
            _dbContext.Audios.Update(audio);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync(new GetAudioCacheOptions(command.AudioId), cancellationToken);
                
            return Result<ImageUploadResponse>.Success(new ImageUploadResponse
            {
                Url = string.Format(MediaLinkInvariants.AudioPictureUrl, blobName)
            });
        }
    }
}