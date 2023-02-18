using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Common.Dtos;
using Audiochan.Common.Interfaces;
using Audiochan.Common.Services;
using Audiochan.Core.Features.Audios.Exceptions;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Audiochan.Core.Features.Audios.Commands.UpdatePicture
{
    public class UpdateAudioPictureCommand : AuthCommandRequest<ImageUploadResponse>, IImageData
    {
        public UpdateAudioPictureCommand(long audioId, string? data, ClaimsPrincipal user) : base(user)
        {
            AudioId = audioId;
            Data = data;
        }

        public long AudioId { get; }
        public string? Data { get; }
    }

    public class UpdateAudioPictureCommandHandler : IRequestHandler<UpdateAudioPictureCommand, ImageUploadResponse>
    {
        private readonly IDistributedCache _cache;
        private readonly IImageService _imageService;
        private readonly IRandomIdGenerator _randomIdGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAudioPictureCommandHandler(IImageService imageService,
            IRandomIdGenerator randomIdGenerator, IUnitOfWork unitOfWork, IDistributedCache cache)
        {
            _imageService = imageService;
            _randomIdGenerator = randomIdGenerator;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<ImageUploadResponse> Handle(UpdateAudioPictureCommand command,
            CancellationToken cancellationToken)
        {
            var currentUserId = command.GetUserId();

            var audio = await _unitOfWork.Audios.FindAsync(command.AudioId, cancellationToken);

            if (audio == null)
                throw new AudioNotFoundException(command.AudioId);

            if (audio.UserId != currentUserId)
                throw new AudioNotFoundException(command.AudioId);

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
                
            return new ImageUploadResponse
            {
                Url = MediaLinkConstants.AUDIO_PICTURE + blobName
            };
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