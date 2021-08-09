using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.FavoritePlaylists.SetFavoritePlaylist
{
    public record SetFavoritePlaylistCommand(long PlaylistId, long UserId, bool IsFavoriting) : IRequest<Result<bool>>
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

            queryable = UserHelpers.IsValidId(command.UserId)
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
        
        private Task<bool> Favorite(Playlist target, long userId)
        {
            var favoriter = target.Favorited.FirstOrDefault(f => f.UserId == userId);

            if (favoriter is null)
            {
                favoriter = new FavoritePlaylist
                {
                    PlaylistId = target.Id,
                    UserId = userId,
                };
                
                target.Favorited.Add(favoriter);
            }

            return Task.FromResult(true);
        }
        
        private Task<bool> Unfavorite(Playlist target, long userId)
        {
            var favoriter = target.Favorited.FirstOrDefault(f => f.UserId == userId);

            if (favoriter is not null)
            {
                target.Favorited.Remove(favoriter);
            }

            return Task.FromResult(false);
        }
    }
}