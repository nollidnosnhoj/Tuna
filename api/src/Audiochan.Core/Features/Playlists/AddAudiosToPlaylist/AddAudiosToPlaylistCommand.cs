using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Playlists.AddAudiosToPlaylist
{
    public record AddAudiosToPlaylistCommand(Guid PlaylistId, List<Guid> AudioIds) : IRequest<Result>;
    
    public class AddAudiosToPlaylistCommandHandler : IRequestHandler<AddAudiosToPlaylistCommand, Result>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public AddAudiosToPlaylistCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(AddAudiosToPlaylistCommand request, CancellationToken cancellationToken)
        {
            var playlist = await _unitOfWork.Playlists
                .LoadWithAudios(request.PlaylistId, request.AudioIds, cancellationToken);

            if (playlist is null)
            {
                return Result.NotFound<Playlist>();
            }

            if (playlist.UserId != _currentUserService.GetUserId())
            {
                return Result.Forbidden();
            }

            if (!await CheckIfAudioIdsAreValid(request.AudioIds, cancellationToken))
            {
                return Result.BadRequest("AudioIds are invalid.");
            }

            foreach (var audioId in request.AudioIds)
            {
                if (playlist.Audios.All(x => x.AudioId != audioId))
                {
                    playlist.Audios.Add(new PlaylistAudio
                    {
                        PlaylistId = playlist.Id,
                        AudioId = audioId
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        private async Task<bool> CheckIfAudioIdsAreValid(List<Guid> audioIds, CancellationToken cancellationToken)
        {
            var audioCount = await _unitOfWork.Audios
                .CountAsync(x => audioIds.Contains(x.Id)
                    && x.Visibility == Visibility.Public, cancellationToken);

            return audioCount == audioIds.Count;
        }
    }
}