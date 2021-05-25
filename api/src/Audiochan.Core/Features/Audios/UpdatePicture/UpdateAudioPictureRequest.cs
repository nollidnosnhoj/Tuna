using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Features.Audios.UpdateAudio;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.UpdatePicture
{
    public class UpdateAudioPictureRequest : IRequest<IResult<AudioDetailViewModel>>
    {
        [JsonIgnore] public Guid AudioId { get; set; }
        public string Data { get; init; } = null!;
    }

    public class UpdateAudioPictureRequestHandler : IRequestHandler<UpdateAudioPictureRequest, IResult<AudioDetailViewModel>>
    {
        private readonly IAudioRepository _audioRepository;
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
            IDateTimeProvider dateTimeProvider, IAudioRepository audioRepository)
        {
            _storageSettings = options.Value;
            _dbContext = dbContext;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _imageService = imageService;
            _dateTimeProvider = dateTimeProvider;
            _audioRepository = audioRepository;
        }

        public async Task<IResult<AudioDetailViewModel>> Handle(UpdateAudioPictureRequest request,
            CancellationToken cancellationToken)
        {
            var container = string.Join('/', _storageSettings.Image.Container, "audios");
            var currentUserId = _currentUserService.GetUserId();

            var audio = await _audioRepository.GetBySpecAsync(new GetAudioForUpdateSpecification(request.AudioId),
                cancellationToken: cancellationToken);
            
            if (audio == null) 
                return Result<AudioDetailViewModel>.Fail(ResultError.NotFound);
            
            if (!audio.CanModify(currentUserId)) 
                return Result<AudioDetailViewModel>.Fail(ResultError.Forbidden);
            
            var blobName = $"{audio.Id}/{_dateTimeProvider.Now:yyyyMMddHHmmss}.jpg";

            try
            {
                await _imageService.UploadImage(request.Data, container, blobName, cancellationToken);
                
                if (!string.IsNullOrEmpty(audio.Picture))
                    await _storageService.RemoveAsync(_storageSettings.Image.Bucket, container, audio.Picture, cancellationToken);
                
                audio.UpdatePicture(blobName);
                await _dbContext.SaveChangesAsync(cancellationToken);
                
                return Result<AudioDetailViewModel>.Success(audio.MapToDetail());
            }
            catch (Exception)
            {
                await _storageService.RemoveAsync(_storageSettings.Audio.Bucket, container, blobName, cancellationToken);
                throw;
            }
        }
    }
}