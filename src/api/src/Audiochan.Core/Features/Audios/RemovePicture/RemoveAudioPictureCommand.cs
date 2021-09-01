using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.RemovePicture
{
    public record RemoveAudioPictureCommand(long AudioId) : IRequest<Result>;
    
    public class RemoveAudioPictureCommandHandler : IRequestHandler<RemoveAudioPictureCommand, Result>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageUploadService _imageUploadService;
        private readonly ICacheService _cacheService;
        private readonly ApplicationDbContext _dbContext;

        public RemoveAudioPictureCommandHandler(ICurrentUserService currentUserService, 
            IImageUploadService imageUploadService, ICacheService cacheService, ApplicationDbContext dbContext)
        {
            _currentUserService = currentUserService;
            _imageUploadService = imageUploadService;
            _cacheService = cacheService;
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(RemoveAudioPictureCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            var audio = await _dbContext.Audios
                .Include(a => a.User)
                .Include(a => a.Tags)
                .SingleOrDefaultAsync(a => a.Id == request.AudioId, cancellationToken);

            if (audio == null)
                return Result<ImageUploadResponse>.NotFound<Audio>();

            if (audio.UserId != currentUserId)
                return Result<ImageUploadResponse>.Forbidden();

            if (string.IsNullOrEmpty(audio.Picture)) return Result.Success();
            
            await _imageUploadService.RemoveImage(AssetContainerConstants.AudioPictures, audio.Picture, cancellationToken);
            audio.Picture = null;

            await _dbContext.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync(new GetAudioCacheOptions(request.AudioId), cancellationToken);

            return Result.Success();
        }
    }
}