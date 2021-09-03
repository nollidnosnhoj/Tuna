using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Playlists.UpdatePlaylistPicture
{
    public record UpdatePlaylistPictureCommand : IImageData, IRequest<Result<ImageUploadResponse>>
    {
        public long Id { get; init; }
        public string Data { get; init; } = null!;

        public UpdatePlaylistPictureCommand(long id, ImageUploadRequest request)
        {
            Id = id;
            Data = request.Data;
        }
    }
    
    
    public class UpdatePlaylistPictureCommandHandler : IRequestHandler<UpdatePlaylistPictureCommand, Result<ImageUploadResponse>>
    {
        private readonly long _currentUserId;
        private readonly IRandomIdGenerator _randomIdGenerator;
        private readonly IImageUploadService _imageUploadService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePlaylistPictureCommandHandler(IImageUploadService imageUploadService,
            IUnitOfWork unitOfWork, 
            ICurrentUserService currentUserService, 
            IRandomIdGenerator randomIdGenerator)
        {
            _imageUploadService = imageUploadService;
            _unitOfWork = unitOfWork;
            _randomIdGenerator = randomIdGenerator;
            _currentUserId = currentUserService.GetUserId();
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
                await _imageUploadService.UploadImage(request.Data, AssetContainerConstants.PlaylistPictures, blobName, cancellationToken);
                await RemoveOriginalPicture(playlist.Picture, cancellationToken);
                playlist.Picture = blobName;
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ImageUploadResponse>.Success(new ImageUploadResponse
            {
                Url = MediaLinkInvariants.PlaylistPicture + blobName
            });
        }
        
        private async Task RemoveOriginalPicture(string? picture, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(picture))
            {
                await _imageUploadService.RemoveImage(AssetContainerConstants.PlaylistPictures, picture, cancellationToken);
            }
        }
    }
}