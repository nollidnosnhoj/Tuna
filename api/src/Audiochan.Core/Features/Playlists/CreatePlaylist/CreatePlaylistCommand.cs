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

namespace Audiochan.Core.Features.Playlists.CreatePlaylist
{
    public record CreatePlaylistCommand : IRequest<Result<Guid>>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Visibility Visibility { get; set; }
        public List<Guid> AudioIds { get; set; } = new();
    }
    
    public class CreatePlaylistCommandHandler : IRequestHandler<CreatePlaylistCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICurrentUserService _currentUserService;

        public CreatePlaylistCommandHandler(IUnitOfWork unitOfWork, 
            IDateTimeProvider dateTimeProvider, 
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(CreatePlaylistCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();
            var user = await _unitOfWork.Users.LoadAsync(userId, cancellationToken);
            if (user is null) return Result<Guid>.Unauthorized();

            if (!await CheckIfAudioIdsExist(request.AudioIds, cancellationToken))
            {
                return Result<Guid>.BadRequest("Audio ids are invalid.");
            }

            var playlist = new Playlist
            {
                Title = request.Title,
                Description = request.Description,
                Visibility = request.Visibility,
                UserId = userId,
                User = user,
                Audios = request.AudioIds
                    .Select(x => new PlaylistAudio
                    {
                        AudioId = x,
                        Added = _dateTimeProvider.Now
                    })
                    .ToList()
            };

            await _unitOfWork.Playlists.AddAsync(playlist, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(playlist.Id);
        }
        
        private async Task<bool> CheckIfAudioIdsExist(ICollection<Guid> audioIds,
            CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Audios.ExistsAsync(x => audioIds.Contains(x.Id)
                && x.Visibility == Visibility.Public, cancellationToken);
        }
    }
}