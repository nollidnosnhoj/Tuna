using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Audiochan.Core.Features.Playlists.RemoveAudiosFromPlaylist
{
    public record RemoveAudiosFromPlaylistCommand : IRequest<Result>
    {
        public long PlaylistId { get; init; }
        public List<long> PlaylistAudioIds { get; init; }

        public RemoveAudiosFromPlaylistCommand(long playlistId, List<long> playlistAudioIds)
        {
            PlaylistId = playlistId;
            PlaylistAudioIds = playlistAudioIds;
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

    public sealed class LoadPlaylistForAudioRemovalSpecification : Specification<Playlist>
    {
        public LoadPlaylistForAudioRemovalSpecification(long playlistId, ICollection<long> playlistAudioIds)
        {
            Query.Include(p => p.PlaylistAudios
                .Where(pa => playlistAudioIds.Contains(pa.Id)));
            Query.Where(p => p.Id == playlistId);
        }
    }

    public class RemoveAudiosFromPlaylistCommandHandler : IRequestHandler<RemoveAudiosFromPlaylistCommand, Result>
    {
        private readonly long _currentUserId;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveAudiosFromPlaylistCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveAudiosFromPlaylistCommand request, CancellationToken cancellationToken)
        {
            var spec = new LoadPlaylistForAudioRemovalSpecification(request.PlaylistId, request.PlaylistAudioIds);
            var playlist = await _unitOfWork.Playlists.GetFirstAsync(spec, cancellationToken);

            if (playlist is null)
                return Result.NotFound("Playlist was not found.");
            if (playlist.UserId != _currentUserId)
                return Result.Forbidden();
            if (playlist.PlaylistAudios.Count == 0)
                return Result.NotFound("Audios was not found in playlist.");

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var id in request.PlaylistAudioIds)
            {
                var playlistAudio = playlist.PlaylistAudios.FirstOrDefault(x => x.Id == id);
                if (playlistAudio is not null)
                    playlist.PlaylistAudios.Remove(playlistAudio);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}