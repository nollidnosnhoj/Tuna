using System;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.UpdatePicture
{
    public class UpdateAudioPictureRequest : IRequest<IResult<string>>
    {
        [JsonIgnore] public Guid AudioId { get; set; }
        public string Data { get; init; }
    }

    public class UpdateAudioPictureRequestHandler : IRequestHandler<UpdateAudioPictureRequest, IResult<string>>
    {
        private readonly MediaStorageSettings _storageSettings;
        private readonly IApplicationDbContext _dbContext;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageService _imageService;

        public UpdateAudioPictureRequestHandler(IOptions<MediaStorageSettings> options,
            IApplicationDbContext dbContext,
            IStorageService storageService,
            ICurrentUserService currentUserService,
            IImageService imageService,
            IDateTimeProvider dateTimeProvider)
        {
            _storageSettings = options.Value;
            _dbContext = dbContext;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _imageService = imageService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IResult<string>> Handle(UpdateAudioPictureRequest request,
            CancellationToken cancellationToken)
        {
            var container = string.Join('/', _storageSettings.Image.Container, "audios");
            var currentUserId = _currentUserService.GetUserId();
            var audio = await _dbContext.Audios
                .FindAsync(new object[]{ request.AudioId }, cancellationToken);
            if (audio == null) 
                return Result<string>.Fail(ResultError.NotFound);
            if (!audio.CanModify(currentUserId)) 
                return Result<string>.Fail(ResultError.Forbidden);
            
            var blobName = $"{audio.Id}/{_dateTimeProvider.Now:yyyyMMddHHmmss}.jpg";

            try
            {
                var response = await _imageService.UploadImage(request.Data, container, blobName, cancellationToken);
                
                // TODO: Maybe instead of deleting it, set a expiration for the old picture object.
                if (!string.IsNullOrEmpty(audio.Picture))
                    await _storageService.RemoveAsync(_storageSettings.Audio.Bucket, audio.Picture, cancellationToken);
                
                audio.UpdatePicture(blobName);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return Result<string>.Success(response.Url);
            }
            catch (Exception)
            {
                await _storageService.RemoveAsync(_storageSettings.Audio.Bucket, container, blobName, cancellationToken);
                throw;
            }
        }
    }
}