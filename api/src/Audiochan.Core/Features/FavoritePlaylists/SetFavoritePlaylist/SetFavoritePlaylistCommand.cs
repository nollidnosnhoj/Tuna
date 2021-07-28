using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.FavoritePlaylists.SetFavoritePlaylist
{
    public record SetFavoritePlaylistCommand(Guid PlaylistId, string UserId, bool IsFavoriting) : IRequest<Result<bool>>
    {
    }

    public class SetFavoritePlaylistCommandHandler : IRequestHandler<SetFavoritePlaylistCommand, Result<bool>>
    {
        private readonly ApplicationDbContext _unitOfWork;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SetFavoritePlaylistCommandHandler(ApplicationDbContext unitOfWork, IDateTimeProvider dateTimeProvider)
        {
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result<bool>> Handle(SetFavoritePlaylistCommand command, CancellationToken cancellationToken)
        {
            var queryable = _unitOfWork.Playlists
                .IgnoreQueryFilters()
                .Where(a => a.Id == command.PlaylistId);

            queryable = !string.IsNullOrEmpty(command.UserId) 
                ? queryable.Include(a => 
                    a.Favorited.Where(fa => fa.UserId == command.UserId)) 
                : queryable.Include(a => a.Favorited);

            var playlist = await queryable.SingleOrDefaultAsync(cancellationToken);

            if (playlist == null)
                return Result<bool>.NotFound<Playlist>();

            var isFavoriting = command.IsFavoriting
                ? await Favorite(playlist, command.UserId)
                : await Unfavorite(playlist, command.UserId);

            _unitOfWork.Playlists.Update(playlist);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(isFavoriting);
        }
        
        private Task<bool> Favorite(Playlist target, string userId)
        {
            var favoriter = target.Favorited.FirstOrDefault(f => f.UserId == userId);

            if (favoriter is null)
            {
                favoriter = new FavoritePlaylist
                {
                    PlaylistId = target.Id,
                    UserId = userId,
                    FavoriteDate = _dateTimeProvider.Now
                };
                
                target.Favorited.Add(favoriter);
            }
            else if (favoriter.UnfavoriteDate is not null)
            {
                favoriter.FavoriteDate = _dateTimeProvider.Now;
                favoriter.UnfavoriteDate = null;
            }
            
            return Task.FromResult(true);
        }
        
        private Task<bool> Unfavorite(Playlist target, string userId)
        {
            var favoriter = target.Favorited.FirstOrDefault(f => f.UserId == userId);

            if (favoriter is not null)
            {
                favoriter.UnfavoriteDate = _dateTimeProvider.Now;
            }

            return Task.FromResult(false);
        }
    }
}