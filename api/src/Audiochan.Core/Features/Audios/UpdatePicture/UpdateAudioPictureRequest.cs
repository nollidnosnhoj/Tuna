using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.UpdateAudio;
using MediatR;
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
        private readonly MediaStorageSettings _storageSettings;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAudioPictureRequestHandler(IOptions<MediaStorageSettings> options,
            IStorageService storageService,
            ICurrentUserService currentUserService,
            IImageService imageService,
            IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
        {
            _storageSettings = options.Value;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _imageService = imageService;
            _dateTimeProvider = dateTimeProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult<AudioDetailViewModel>> Handle(UpdateAudioPictureRequest request,
            CancellationToken cancellationToken)
        {
            var container = string.Join('/', _storageSettings.Image.Container, "audios");
            var currentUserId = _currentUserService.GetUserId();
            
            var audio = await _unitOfWork.Audios.GetBySpecAsync(new GetAudioForUpdateSpecification(request.AudioId),
                cancellationToken: cancellationToken);
        
            if (audio == null) 
                return Result<AudioDetailViewModel>.Fail(ResultError.NotFound);
        
            if (!audio.CanModify(currentUserId)) 
                return Result<AudioDetailViewModel>.Fail(ResultError.Forbidden);
        
            var blobName = $"{audio.Id}/{_dateTimeProvider.Now:yyyyMMddHHmmss}.jpg";
            
            await _imageService.UploadImage(request.Data, container, blobName, cancellationToken);
            
            if (!string.IsNullOrEmpty(audio.Picture))
                await _storageService.RemoveAsync(_storageSettings.Image.Bucket, container, audio.Picture, cancellationToken);
            
            audio.UpdatePicture(blobName);
            _unitOfWork.Audios.Update(audio);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AudioDetailViewModel>.Success(audio.MapToDetail());
        }
    }
}