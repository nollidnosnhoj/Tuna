using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Playlists.GetPlaylist
{
    public record GetPlaylistQuery(long Id) : IRequest<PlaylistViewModel?>;
    
    public class GetPlaylistQueryHandler : IRequestHandler<GetPlaylistQuery, PlaylistViewModel?>
    {
        private readonly long _currentUserId;
        private readonly ApplicationDbContext _unitOfWork;

        public GetPlaylistQueryHandler(ICurrentUserService currentUserService, ApplicationDbContext unitOfWork)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
        }

        public async Task<PlaylistViewModel?> Handle(GetPlaylistQuery request, CancellationToken cancellationToken)
        {
            var playlist = await _unitOfWork.Playlists
                .AsNoTracking()
                .Where(x => x.Id == request.Id)
                .Where(p => p.UserId == _currentUserId || p.Visibility == Visibility.Public)
                .Select(PlaylistMaps.PlaylistToDetailFunc)
                .SingleOrDefaultAsync(cancellationToken);
            
            if (playlist == null || !CanAccessPrivatePlaylist(playlist)) return null;

            return playlist;
        }
        
        private bool CanAccessPrivatePlaylist(PlaylistViewModel playlist)
        {
            return _currentUserId == playlist.User.Id || playlist.Visibility != Visibility.Private;
        }
    }
}