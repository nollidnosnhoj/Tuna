using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Playlists.GetPlaylistAudios;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Playlists.GetPlaylistDetail
{
    public record GetPlaylistDetailQuery(Guid Id, bool IncludeAudios) : IRequest<PlaylistDetailViewModel?>;
    
    public class GetPlaylistDetailQueryHandler : IRequestHandler<GetPlaylistDetailQuery, PlaylistDetailViewModel?>
    {
        private readonly string _currentUserId;
        private readonly ApplicationDbContext _unitOfWork;

        public GetPlaylistDetailQueryHandler(ICurrentUserService currentUserService, ApplicationDbContext unitOfWork)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
        }

        public async Task<PlaylistDetailViewModel?> Handle(GetPlaylistDetailQuery request, CancellationToken cancellationToken)
        {
            var playlist = await _unitOfWork.Playlists
                .AsNoTracking()
                .Include(x => x.Tags)
                .Include(x => x.User)
                .Where(x => x.Id == request.Id)
                .Select(PlaylistMaps.PlaylistToDetailFunc)
                .SingleOrDefaultAsync(cancellationToken);
            
            if (playlist == null || !CanAccessPrivatePlaylist(playlist)) return null;

            if (!request.IncludeAudios) return playlist;

            var audios = await _unitOfWork.PlaylistAudios
                .Include(pa => pa.Audio)
                .Where(pa => pa.PlaylistId == request.Id)
                .OrderByDescending(pa => pa.Added)
                .Select(pa => pa.Audio)
                .Select(AudioMaps.AudioToView)
                .Take(100)
                .ToListAsync(cancellationToken);

            return playlist with {Audios = audios};
        }
        
        private bool CanAccessPrivatePlaylist(PlaylistDetailViewModel playlist)
        {
            return _currentUserId == playlist.User.Id || playlist.Visibility != Visibility.Private;
        }
    }
}