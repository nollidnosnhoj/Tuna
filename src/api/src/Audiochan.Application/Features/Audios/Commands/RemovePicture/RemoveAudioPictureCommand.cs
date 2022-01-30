using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Dtos.Responses;
using Audiochan.Application.Commons.Services;
using Audiochan.Application.Persistence;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Audiochan.Application.Features.Audios.Commands.RemovePicture
{
    public record RemoveAudioPictureCommand(long AudioId) : ICommandRequest<Result>;
    
    public class RemoveAudioPictureCommandHandler : IRequestHandler<RemoveAudioPictureCommand, Result>
    {
        private readonly IDistributedCache _cache;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveAudioPictureCommandHandler(ICurrentUserService currentUserService, 
            IImageService imageService, IUnitOfWork unitOfWork, IDistributedCache cache)
        {
            _currentUserService = currentUserService;
            _imageService = imageService;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result> Handle(RemoveAudioPictureCommand request, CancellationToken cancellationToken)
        {
            _currentUserService.User.TryGetUserId(out var currentUserId);

            var audio = await _unitOfWork.Audios.FindAsync(request.AudioId, cancellationToken);

            if (audio == null)
                return Result<ImageUploadResponse>.NotFound<Audio>();

            if (audio.UserId != currentUserId)
                return Result<ImageUploadResponse>.Forbidden();

            if (string.IsNullOrEmpty(audio.Picture)) return Result.Success();
            
            await _imageService.RemoveImage(AssetContainerConstants.AUDIO_PICTURES, audio.Picture, cancellationToken);
            audio.Picture = null;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Audio.GetAudio(request.AudioId), cancellationToken);

            return Result.Success();
        }
    }
}