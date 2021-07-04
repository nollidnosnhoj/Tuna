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
        private readonly ICacheService _cacheService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public GetPlaylistDetailQueryHandler(ICacheService cacheService, ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _cacheService = cacheService;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<PlaylistDetailViewModel?> Handle(GetPlaylistDetailQuery request, CancellationToken cancellationToken)
        {
            var cacheOptions = new GetPlaylistDetailCacheOptions(request.Id);
            
            var ( exists, playlist ) = await _cacheService
                .GetAsync<PlaylistDetailViewModel>(cacheOptions, cancellationToken);

            if (!exists)
            {
                playlist = await _unitOfWork.Playlists.GetPlaylistDetail(request.Id, cancellationToken);
                if (playlist == null) return null;
                await _cacheService.SetAsync(playlist, cacheOptions, cancellationToken);
            }

            return CanAccessPrivatePlaylist(playlist!) ? playlist : null;
        }
        
        private bool CanAccessPrivatePlaylist(PlaylistDetailViewModel playlist)
        {
            var currentUserId = _currentUserService.GetUserId();

            return currentUserId == playlist.User.Id || playlist.Visibility != Visibility.Private;
        }
    }
}