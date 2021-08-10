using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Playlists.GetPlaylistDetail
{
    public record GetPlaylistDetailQuery(long Id) : IRequest<PlaylistViewModel?>;
    
    public class GetPlaylistDetailQueryHandler : IRequestHandler<GetPlaylistDetailQuery, PlaylistViewModel?>
    {
        private readonly long _currentUserId;
        private readonly ApplicationDbContext _unitOfWork;

        public GetPlaylistDetailQueryHandler(ICurrentUserService currentUserService, ApplicationDbContext unitOfWork)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
        }

        public async Task<PlaylistViewModel?> Handle(GetPlaylistDetailQuery request, CancellationToken cancellationToken)
        {
            var playlist = await _unitOfWork.Playlists
                .AsNoTracking()
                .Include(x => x.Tags)
                .Include(x => x.User)
                .Where(x => x.Id == request.Id)
                .FilterVisibility(_currentUserId, FilterVisibilityMode.Unlisted)
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