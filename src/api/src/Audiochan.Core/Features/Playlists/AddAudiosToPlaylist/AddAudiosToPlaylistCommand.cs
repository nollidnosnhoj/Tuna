using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Audiochan.Core.Features.Playlists.AddAudiosToPlaylist
{
    public record AddAudiosToPlaylistCommand : IRequest<Result>
    {
        public long PlaylistId { get; init; }
        public List<long> AudioIds { get; init; }

        public AddAudiosToPlaylistCommand(long playlistId, List<long> audioIds)
        {
            PlaylistId = playlistId;
            AudioIds = audioIds;
        }
    }
    
    public class AddAudiosToPlaylistCommandValidator : AbstractValidator<AddAudiosToPlaylistCommand>
    {
        public AddAudiosToPlaylistCommandValidator()
        {
            RuleFor(x => x.AudioIds)
                .NotEmpty()
                .WithMessage("Audio ids cannot be empty.");
        }
    }

    public class AddAudiosToPlaylistCommandHandler : IRequestHandler<AddAudiosToPlaylistCommand, Result>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public AddAudiosToPlaylistCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddAudiosToPlaylistCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var playlist = await _unitOfWork.Playlists.FindAsync(request.PlaylistId, cancellationToken);

            if (playlist is null)
            {
                return Result.NotFound<Playlist>();
            }

            if (playlist.UserId != currentUserId)
            {
                return Result.Forbidden();
            }

            if (!await CheckIfAudioIdsAreValid(request.AudioIds, cancellationToken))
            {
                return Result.BadRequest("AudioIds are invalid.");
            }

            var playlistAudios = request.AudioIds
                .Select(audioId => new PlaylistAudio
                {
                    PlaylistId = playlist.Id,
                    AudioId = audioId
                })
                .ToList();

            await _unitOfWork.PlaylistAudios.AddRangeAsync(playlistAudios, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        private async Task<bool> CheckIfAudioIdsAreValid(ICollection<long> audioIds, CancellationToken cancellationToken)
        {
            var audioCount = await _unitOfWork.Audios
                .CountAsync(x => audioIds.Contains(x.Id), cancellationToken);

            return audioCount == audioIds.Count;
        }
    }
}