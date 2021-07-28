using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Playlists.AddAudiosToPlaylist
{
    public record AddAudiosToPlaylistCommand : IRequest<Result>
    {
        public Guid PlaylistId { get; init; }
        public List<Guid> AudioIds { get; init; }

        public AddAudiosToPlaylistCommand(Guid playlistId, List<Guid> audioIds)
        {
            PlaylistId = playlistId;
            AudioIds = audioIds;
        }

        public AddAudiosToPlaylistCommand(Guid playlistId, AddAudiosToPlaylistRequest request)
        {
            PlaylistId = playlistId;
            AudioIds = request.AudioIds;
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
        private readonly ApplicationDbContext _unitOfWork;

        public AddAudiosToPlaylistCommandHandler(ApplicationDbContext unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(AddAudiosToPlaylistCommand request, CancellationToken cancellationToken)
        {
            IQueryable<Playlist> queryable = _unitOfWork.Playlists;
            if (request.AudioIds.Count == 0)
            {
                queryable = queryable.Include(x => x.Audios);
            }
            else
            {
                queryable = queryable
                    .Include(x => x.Audios.Where(a => request.AudioIds.Contains(a.AudioId)));
            }

            var playlist = await queryable.SingleOrDefaultAsync(p => p.Id == request.PlaylistId, cancellationToken);

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