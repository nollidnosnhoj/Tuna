using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Audios.GetAudio;
using Audiochan.API.Features.Audios.UpdateAudio;
using Audiochan.API.Mappings;
using Audiochan.Core.Models;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using Audiochan.Core.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.API.Features.Audios.UpdatePicture
{
    public class UpdateAudioPictureRequest : IRequest<Result<AudioDetailViewModel>>
    {
        [JsonIgnore] public Guid AudioId { get; set; }
        public string Data { get; init; } = null!;
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
            
            var audio = await _unitOfWork.Audios.GetAsync(new GetAudioForUpdateSpecification(request.AudioId),
                cancellationToken: cancellationToken);
        
            if (audio == null) 
                return Result<AudioDetailViewModel>.Fail(ResultError.NotFound);
        
            if (!audio.CanModify(currentUserId)) 
                return Result<AudioDetailViewModel>.Fail(ResultError.Forbidden);
        
            var blobName = $"{audio.Id}/{_dateTimeProvider.Now:yyyyMMddHHmmss}.jpg";
            
            await _imageProcessingService.UploadImage(request.Data, container, blobName, cancellationToken);
            
            if (!string.IsNullOrEmpty(audio.Picture))
                await _storageService.RemoveAsync(_storageSettings.Image.Bucket, container, audio.Picture, cancellationToken);
            
            audio.UpdatePicture(blobName);
            _unitOfWork.Audios.Update(audio);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AudioDetailViewModel>.Success(audio.MapToDetail());
        }
    }
}