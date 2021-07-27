using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Playlists.RemoveAudiosFromPlaylist
{
    public record RemoveAudiosFromPlaylistCommand : IRequest<Result>
    {
        public Guid PlaylistId { get; init; }
        public List<Guid> AudioIds { get; init; }

        public RemoveAudiosFromPlaylistCommand(Guid playlistId, List<Guid> audioIds)
        {
            PlaylistId = playlistId;
            AudioIds = audioIds;
        }
        
        public RemoveAudiosFromPlaylistCommand(Guid playlistId,
            RemoveAudiosFromPlaylistRequest request)
        {
            PlaylistId = playlistId;
            AudioIds = request.AudioIds;
        }
    }
    
    public class RemoveAudiosFromPlaylistCommandHandler : IRequestHandler<RemoveAudiosFromPlaylistCommand, Result>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveAudiosFromPlaylistCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveAudiosFromPlaylistCommand request, CancellationToken cancellationToken)
        {
            var playlist = await _unitOfWork.Playlists
                .LoadWithAudios(request.PlaylistId, request.AudioIds, cancellationToken);

            if (playlist is null)
                return Result.NotFound<Playlist>();

            if (playlist.UserId != _currentUserService.GetUserId())
                return Result.Forbidden();

            foreach (var audioId in request.AudioIds)
            {
                var playlistAudio = playlist.Audios.FirstOrDefault(x => x.AudioId == audioId);
                if (playlistAudio != null)
                {
                    playlist.Audios.Remove(playlistAudio);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}