using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Dtos.Responses;
using Audiochan.Application.Commons.Interfaces;
using Audiochan.Application.Commons.Services;
using Audiochan.Application.Persistence;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Audiochan.Application.Features.Audios.Commands.UpdatePicture
{
    public record UpdateAudioPictureCommand(long AudioId, string Data) : IImageData, ICommandRequest<Result<ImageUploadResponse>>
    {
    }

    public class UpdateAudioPictureCommandHandler : IRequestHandler<UpdateAudioPictureCommand, Result<ImageUploadResponse>>
    {
        private readonly IDistributedCache _cache;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageService _imageService;
        private readonly IRandomIdGenerator _randomIdGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAudioPictureCommandHandler(ICurrentUserService currentUserService,
            IImageService imageService,
            IRandomIdGenerator randomIdGenerator, IUnitOfWork unitOfWork, IDistributedCache cache)
        {
            _currentUserService = currentUserService;
            _imageService = imageService;
            _randomIdGenerator = randomIdGenerator;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<ImageUploadResponse>> Handle(UpdateAudioPictureCommand command,
            CancellationToken cancellationToken)
        {
            _currentUserService.User.TryGetUserId(out var currentUserId);

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
                await _imageService.UploadImage(command.Data, AssetContainerConstants.AUDIO_PICTURES, blobName, cancellationToken);
                await RemoveOriginalPicture(audio.Picture, cancellationToken);
                audio.Picture = blobName;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Audio.GetAudio(command.AudioId), cancellationToken);
                
            return Result<ImageUploadResponse>.Success(new ImageUploadResponse
            {
                Url = MediaLinkConstants.AUDIO_PICTURE + blobName
            });
        }

        private async Task RemoveOriginalPicture(string? picture, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(picture))
            {
                await _imageService.RemoveImage(AssetContainerConstants.AUDIO_PICTURES, picture, cancellationToken);
            }
        }
    }
}