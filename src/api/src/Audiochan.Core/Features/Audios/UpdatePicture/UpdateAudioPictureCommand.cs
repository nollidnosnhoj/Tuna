using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using Audiochan.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.UpdatePicture
{
    public record UpdateAudioPictureCommand(long AudioId, string Data = "") : IImageData, IRequest<Result<ImageUploadResponse>>
    {
    }

    public class UpdateAudioCommandHandler : IRequestHandler<UpdateAudioPictureCommand, Result<ImageUploadResponse>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageUploadService _imageUploadService;
        private readonly ICacheService _cacheService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IRandomIdGenerator _randomIdGenerator;

        public UpdateAudioCommandHandler(ICurrentUserService currentUserService,
            IImageUploadService imageUploadService,
            ICacheService cacheService, 
            ApplicationDbContext dbContext, 
            IRandomIdGenerator randomIdGenerator)
        {
            _currentUserService = currentUserService;
            _imageUploadService = imageUploadService;
            _cacheService = cacheService;
            _dbContext = dbContext;
            _randomIdGenerator = randomIdGenerator;
        }

        public async Task<Result<ImageUploadResponse>> Handle(UpdateAudioPictureCommand command,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            var audio = await _dbContext.Audios
                .Include(a => a.User)
                .Include(a => a.Tags)
                .SingleOrDefaultAsync(a => a.Id == command.AudioId, cancellationToken);

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
                await _imageUploadService.UploadImage(command.Data, AssetContainerConstants.AudioPictures, blobName, cancellationToken);
                await RemoveOriginalPicture(audio.Picture, cancellationToken);
                audio.Picture = blobName;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync(new GetAudioCacheOptions(command.AudioId), cancellationToken);
                
            return Result<ImageUploadResponse>.Success(new ImageUploadResponse
            {
                Url = string.Format(MediaLinkInvariants.AudioPictureUrl, blobName)
            });
        }

        private async Task RemoveOriginalPicture(string? picture, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(picture))
            {
                await _imageUploadService.RemoveImage(AssetContainerConstants.AudioPictures, picture, cancellationToken);
            }
        }
    }
}