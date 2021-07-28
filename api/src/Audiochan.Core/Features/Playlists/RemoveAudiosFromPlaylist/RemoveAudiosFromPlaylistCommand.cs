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
    
    public class RemoveAudiosFromPlaylistCommandValidator : AbstractValidator<RemoveAudiosFromPlaylistCommand>
    {
        public RemoveAudiosFromPlaylistCommandValidator()
        {
            RuleFor(x => x.AudioIds)
                .NotEmpty()
                .WithMessage("Audio ids cannot be empty.");
        }
    }
    
    public class RemoveAudiosFromPlaylistCommandHandler : IRequestHandler<RemoveAudiosFromPlaylistCommand, Result>
    {
        private readonly string _currentUserId;
        private readonly ApplicationDbContext _unitOfWork;

        public RemoveAudiosFromPlaylistCommandHandler(ICurrentUserService currentUserService, ApplicationDbContext unitOfWork)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveAudiosFromPlaylistCommand request, CancellationToken cancellationToken)
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
                return Result.NotFound<Playlist>();

            if (playlist.UserId != _currentUserId)
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