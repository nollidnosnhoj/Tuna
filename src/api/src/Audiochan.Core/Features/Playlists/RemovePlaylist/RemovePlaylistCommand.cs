using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Playlists.RemovePlaylist
{
    public record RemovePlaylistCommand(long Id) : IRequest<Result>;
    
    

    public class RemovePlaylistCommandHandler : IRequestHandler<RemovePlaylistCommand, Result>
    {
        private readonly long _currentUserId;
        private readonly ApplicationDbContext _unitOfWork;
        private readonly IImageUploadService _imageUploadService;

        public RemovePlaylistCommandHandler(ICurrentUserService currentUserService, ApplicationDbContext unitOfWork, 
            IImageUploadService imageUploadService)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
            _imageUploadService = imageUploadService;
        }

        public async Task<Result> Handle(RemovePlaylistCommand request, CancellationToken cancellationToken)
        {
            var playlist = await _unitOfWork.Playlists
                .Include(a => a.Audios)
                .SingleOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

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