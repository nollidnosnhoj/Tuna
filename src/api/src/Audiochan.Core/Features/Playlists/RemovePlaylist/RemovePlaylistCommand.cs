using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Playlists.RemovePlaylist
{
    public record RemovePlaylistCommand(long Id) : IRequest<Result>;
    
    

    public class RemovePlaylistCommandHandler : IRequestHandler<RemovePlaylistCommand, Result>
    {
        private readonly long _currentUserId;
        private readonly IStorageService _storageService;
        private readonly MediaStorageSettings _storageSettings;
        private readonly ApplicationDbContext _unitOfWork;

        public RemovePlaylistCommandHandler(ICurrentUserService currentUserService, ApplicationDbContext unitOfWork, IStorageService storageService, IOptions<MediaStorageSettings> storageSettings)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _storageSettings = storageSettings.Value;
        }

        public async Task<Result> Handle(RemovePlaylistCommand request, CancellationToken cancellationToken)
        {
            var playlist = await _unitOfWork.Playlists
                .Include(a => a.Audios)
                .SingleOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (playlist is null)
                return Result.NotFound<Playlist>();

            if (playlist.UserId != _currentUserId)
                return Result.Forbidden();

            _unitOfWork.Playlists.Remove(playlist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            if (!string.IsNullOrEmpty(playlist.Picture))
            {
                await _storageService.RemoveAsync(_storageSettings.Image.Bucket,
                    string.Join('/', _storageSettings.Image.Container, "playlists"),
                    playlist.Picture,
                    cancellationToken);
            }

            return Result.Success();
        }
    }
}