using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Audios.GetAudio;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Audios.UpdatePicture
{
    public record UpdateAudioPictureCommand(long AudioId, string Data) : IImageData, IRequest<Result<ImageUploadResponse>>
    {
    }

    public class UpdateAudioCommandHandler : IRequestHandler<UpdateAudioPictureCommand, Result<ImageUploadResponse>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageService _imageService;
        private readonly ICacheService _cacheService;
        private readonly IRandomIdGenerator _randomIdGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAudioCommandHandler(ICurrentUserService currentUserService,
            IImageService imageService,
            ICacheService cacheService, 
            IRandomIdGenerator randomIdGenerator, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _imageService = imageService;
            _cacheService = cacheService;
            _randomIdGenerator = randomIdGenerator;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ImageUploadResponse>> Handle(UpdateAudioPictureCommand command,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            var audio = await _unitOfWork.Audios.FindAsync(command.AudioId, cancellationToken);

            if (audio == null)
                return Result<ImageUploadResponse>.NotFound<Audio>();

            if (audio.UserId != currentUserId)
                return Result<ImageUploadResponse>.Forbidden();

            var blobName = string.Empty;
            if (string.IsNullOrEmpty(command.Data))
            {
                await RemoveOriginalPicture(audio.Picture, cancellationToken);
                audio.Picture = null;
            }
            else
            {
                blobName = $"{await _randomIdGenerator.GenerateAsync(size: 15)}.jpg";
                await _imageService.UploadImage(command.Data, AssetContainerConstants.AudioPictures, blobName, cancellationToken);
                await RemoveOriginalPicture(audio.Picture, cancellationToken);
                audio.Picture = blobName;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync(new GetAudioCacheOptions(command.AudioId), cancellationToken);
                
            return Result<ImageUploadResponse>.Success(new ImageUploadResponse
            {
                Url = MediaLinkConstants.AudioPicture + blobName
            });
        }

        private async Task RemoveOriginalPicture(string? picture, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(picture))
            {
                await _imageService.RemoveImage(AssetContainerConstants.AudioPictures, picture, cancellationToken);
            }
        }
    }
}