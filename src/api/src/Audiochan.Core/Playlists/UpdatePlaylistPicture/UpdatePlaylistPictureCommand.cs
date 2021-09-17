using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Playlists.UpdatePlaylistPicture
{
    public record UpdatePlaylistPictureCommand(long Id, string Data) 
        : IImageData, IRequest<Result<ImageUploadResponse>>;
    
    
    public class UpdatePlaylistPictureCommandHandler : IRequestHandler<UpdatePlaylistPictureCommand, Result<ImageUploadResponse>>
    {
        private readonly long _currentUserId;
        private readonly IRandomIdGenerator _randomIdGenerator;
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePlaylistPictureCommandHandler(IImageService imageService,
            IUnitOfWork unitOfWork, 
            IAuthService authService, 
            IRandomIdGenerator randomIdGenerator)
        {
            _imageService = imageService;
            _unitOfWork = unitOfWork;
            _randomIdGenerator = randomIdGenerator;
            _currentUserId = authService.GetUserId();
        }
        
        public async Task<Result<ImageUploadResponse>> Handle(UpdatePlaylistPictureCommand request, CancellationToken cancellationToken)
        {
            var playlist = await _unitOfWork.Playlists.FindAsync(request.Id, cancellationToken);

            if (playlist == null)
                return Result<ImageUploadResponse>.NotFound<Playlist>();

            if (playlist.UserId != _currentUserId)
                return Result<ImageUploadResponse>.Forbidden();
            
            var blobName = string.Empty;
            if (string.IsNullOrEmpty(request.Data))
            {
                await RemoveOriginalPicture(playlist.Picture, cancellationToken);
                playlist.Picture = null;
            }
            else
            {
                blobName = $"{await _randomIdGenerator.GenerateAsync(size: 15)}.jpg";
                await _imageService.UploadImage(request.Data, AssetContainerConstants.PLAYLIST_PICTURES, blobName, cancellationToken);
                await RemoveOriginalPicture(playlist.Picture, cancellationToken);
                playlist.Picture = blobName;
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ImageUploadResponse>.Success(new ImageUploadResponse
            {
                Url = MediaLinkConstants.PLAYLIST_PICTURE + blobName
            });
        }
        
        private async Task RemoveOriginalPicture(string? picture, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(picture))
            {
                await _imageService.RemoveImage(AssetContainerConstants.PLAYLIST_PICTURES, picture, cancellationToken);
            }
        }
    }
}