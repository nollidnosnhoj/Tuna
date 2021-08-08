using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Playlists.RemoveAudiosFromPlaylist
{
    public record RemoveAudiosFromPlaylistCommand : IRequest<Result>
    {
        public Guid PlaylistId { get; init; }
        public List<Guid> PlaylistAudioIds { get; init; }

        public RemoveAudiosFromPlaylistCommand(Guid playlistId, List<Guid> playlistAudioIds)
        {
            PlaylistId = playlistId;
            PlaylistAudioIds = playlistAudioIds;
        }
        
        public RemoveAudiosFromPlaylistCommand(Guid playlistId,
            RemoveAudiosFromPlaylistRequest request)
        {
            PlaylistId = playlistId;
            PlaylistAudioIds = request.PlaylistAudioIds;
        }
    }
    
    public class RemoveAudiosFromPlaylistCommandValidator : AbstractValidator<RemoveAudiosFromPlaylistCommand>
    {
        public RemoveAudiosFromPlaylistCommandValidator()
        {
            RuleFor(x => x.PlaylistAudioIds)
                .NotEmpty()
                .WithMessage("Audio ids cannot be empty.");
        }
    }
    
    public class RemoveAudiosFromPlaylistCommandHandler : IRequestHandler<RemoveAudiosFromPlaylistCommand, Result>
    {
        private readonly long _currentUserId;
        private readonly ApplicationDbContext _unitOfWork;

        public RemoveAudiosFromPlaylistCommandHandler(ICurrentUserService currentUserService, ApplicationDbContext unitOfWork)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveAudiosFromPlaylistCommand request, CancellationToken cancellationToken)
        {
            var playlist = await _unitOfWork.Playlists
                .Include(p =>
                    p.Audios.Where(pa => request.PlaylistAudioIds.Contains(pa.Id)))
                .Where(p => p.Id == request.PlaylistId)
                .SingleOrDefaultAsync(cancellationToken);

            if (playlist is null)
                return Result.NotFound("Playlist was not found.");
            if (playlist.UserId != _currentUserId)
                return Result.Forbidden();
            if (playlist.Audios.Count == 0)
                return Result.NotFound("Audios was not found in playlist.");

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var id in request.PlaylistAudioIds)
            {
                var playlistAudio = playlist.Audios.FirstOrDefault(x => x.Id == id);
                if (playlistAudio is not null)
                    playlist.Audios.Remove(playlistAudio);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}