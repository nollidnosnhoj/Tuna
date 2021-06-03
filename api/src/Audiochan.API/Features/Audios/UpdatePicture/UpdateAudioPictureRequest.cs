using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Audios.GetAudio;
using Audiochan.API.Mappings;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models;
using Audiochan.Core.Services;
using Audiochan.Core.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.API.Features.Audios.UpdatePicture
{
    public class UpdateAudioPictureRequest : IRequest<Result<AudioDetailViewModel>>
    {
        [JsonIgnore] public Guid AudioId { get; set; }
        public string Data { get; set; } = string.Empty;
    }

    public class UpdateAudioPictureRequestHandler : IRequestHandler<UpdateAudioPictureRequest, Result<AudioDetailViewModel>>
    {
        private readonly MediaStorageSettings _storageSettings;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAudioPictureRequestHandler(IOptions<MediaStorageSettings> options,
            IStorageService storageService,
            ICurrentUserService currentUserService,
            IImageProcessingService imageProcessingService,
            IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
        {
            _storageSettings = options.Value;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _imageProcessingService = imageProcessingService;
            _dateTimeProvider = dateTimeProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AudioDetailViewModel>> Handle(UpdateAudioPictureRequest request,
            CancellationToken cancellationToken)
        {
            var container = string.Join('/', _storageSettings.Image.Container, "audios");
            var currentUserId = _currentUserService.GetUserId();
            
            var audio = await _unitOfWork.Audios
                .Include(x => x.Tags)
                .Include(x => x.User)
                .Where(x => x.Id == request.AudioId)
                .SingleOrDefaultAsync(cancellationToken);
        
            if (audio == null) 
                return Result<AudioDetailViewModel>.Fail(ResultError.NotFound);
        
            if (!audio.CanModify(currentUserId)) 
                return Result<AudioDetailViewModel>.Fail(ResultError.Forbidden);
        
            var blobName = $"{audio.Id}/{_dateTimeProvider.Now:yyyyMMddHHmmss}.jpg";
            
            await _imageProcessingService.UploadImage(request.Data, container, blobName, cancellationToken);
            
            if (!string.IsNullOrEmpty(audio.PictureBlobName))
                await _storageService.RemoveAsync(_storageSettings.Image.Bucket, container, audio.PictureBlobName, cancellationToken);
            
            audio.UpdatePicture(blobName);
            _unitOfWork.Audios.Update(audio);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AudioDetailViewModel>.Success(audio.MapToDetail());
        }
    }
}