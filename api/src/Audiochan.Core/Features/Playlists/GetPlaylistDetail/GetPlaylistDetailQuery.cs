using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Playlists.GetPlaylistAudios;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Playlists.GetPlaylistDetail
{
    public record GetPlaylistDetailQuery(Guid Id, bool IncludeAudios) : IRequest<PlaylistDetailViewModel?>;
    
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
            
            if (playlist == null || !CanAccessPrivatePlaylist(playlist)) return null;

            if (!request.IncludeAudios) return playlist;
            
            var audios = await _unitOfWork.Playlists
                .GetAudios(new GetPlaylistAudiosQuery(playlist.Id)
                {
                    Page = 1,
                    Size = 100
                }, cancellationToken);

            return playlist with {Audios = audios.Items};
        }
        
        private bool CanAccessPrivatePlaylist(PlaylistDetailViewModel playlist)
        {
            var currentUserId = _currentUserService.GetUserId();

            return currentUserId == playlist.User.Id || playlist.Visibility != Visibility.Private;
        }
    }
}