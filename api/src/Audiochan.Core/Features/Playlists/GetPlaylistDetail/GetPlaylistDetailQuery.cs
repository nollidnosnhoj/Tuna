using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Playlists.GetPlaylistDetail
{
    public record GetPlaylistDetailQuery(Guid Id) : IRequest<PlaylistDetailViewModel?>;
    
    public class GetPlaylistDetailQueryHandler : IRequestHandler<GetPlaylistDetailQuery, PlaylistDetailViewModel?>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public GetPlaylistDetailQueryHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<PlaylistDetailViewModel?> Handle(GetPlaylistDetailQuery request, CancellationToken cancellationToken)
        {
            var playlist = await _unitOfWork.Playlists.Get(request.Id, cancellationToken);
            
            if (playlist == null) return null;

            return CanAccessPrivatePlaylist(playlist!) ? playlist : null;
        }
        
        private bool CanAccessPrivatePlaylist(PlaylistDetailViewModel playlist)
        {
            var currentUserId = _currentUserService.GetUserId();

            return currentUserId == playlist.User.Id || playlist.Visibility != Visibility.Private;
        }
    }
}