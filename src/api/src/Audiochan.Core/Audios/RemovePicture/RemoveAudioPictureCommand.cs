using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Audios.GetAudio;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Audios.RemovePicture
{
    public record RemoveAudioPictureCommand(long AudioId) : IRequest<Result>;
    
    public class RemoveAudioPictureCommandHandler : IRequestHandler<RemoveAudioPictureCommand, Result>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageUploadService _imageUploadService;
        private readonly ICacheService _cacheService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveAudioPictureCommandHandler(ICurrentUserService currentUserService, 
            IImageUploadService imageUploadService, ICacheService cacheService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _imageUploadService = imageUploadService;
            _cacheService = cacheService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveAudioPictureCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            var audio = await _unitOfWork.Audios.FindAsync(request.AudioId, cancellationToken);

            if (audio == null)
                return Result<ImageUploadResponse>.NotFound<Audio>();

            if (audio.UserId != currentUserId)
                return Result<ImageUploadResponse>.Forbidden();

            if (string.IsNullOrEmpty(audio.Picture)) return Result.Success();
            
            await _imageUploadService.RemoveImage(AssetContainerConstants.AudioPictures, audio.Picture, cancellationToken);
            audio.Picture = null;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync(new GetAudioCacheOptions(request.AudioId), cancellationToken);

            return Result.Success();
        }
    }
}