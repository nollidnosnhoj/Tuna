using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Playlists.RemovePlaylist
{
    public record RemovePlaylistCommand(long Id) : IRequest<Result>;
    
    

    public class RemovePlaylistCommandHandler : IRequestHandler<RemovePlaylistCommand, Result>
    {
        private readonly long _currentUserId;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageUploadService _imageUploadService;

        public RemovePlaylistCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, 
            IImageUploadService imageUploadService)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
            _imageUploadService = imageUploadService;
        }

        public async Task<Result> Handle(RemovePlaylistCommand request, CancellationToken cancellationToken)
        {
            var playlist = await _unitOfWork.Playlists.FindAsync(request.Id, cancellationToken);

            if (playlist is null)
                return Result.NotFound<Playlist>();

            if (playlist.UserId != _currentUserId)
                return Result.Forbidden();

            _unitOfWork.Playlists.Remove(playlist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            if (!string.IsNullOrEmpty(playlist.Picture))
            {
                await _imageUploadService.RemoveImage(AssetContainerConstants.PlaylistPictures, playlist.Picture,
                    cancellationToken);
            }

            return Result.Success();
        }
    }
}