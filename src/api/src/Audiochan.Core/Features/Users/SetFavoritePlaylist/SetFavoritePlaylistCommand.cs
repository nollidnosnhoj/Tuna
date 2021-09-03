using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Users.SetFavoritePlaylist
{
    public record SetFavoritePlaylistCommand(long PlaylistId, long UserId, bool IsFavoriting) : IRequest<Result<bool>>
    {
    }
    
    public sealed class LoadPlaylistForFavoritingSpecification : Specification<Playlist>
    {
        public LoadPlaylistForFavoritingSpecification(long playlistId, long observerId)
        {
            if (observerId > 0)
            {
                Query.Include(a =>
                    a.FavoritePlaylists.Where(fa => fa.UserId == observerId));
            }
            else
            {
                Query.Include(a => a.FavoritePlaylists);
            }

            Query.Where(a => a.Id == playlistId);
        }
    }

    public class SetFavoritePlaylistCommandHandler : IRequestHandler<SetFavoritePlaylistCommand, Result<bool>>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUnitOfWork _unitOfWork;

        public SetFavoritePlaylistCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
        {
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result<bool>> Handle(SetFavoritePlaylistCommand command, CancellationToken cancellationToken)
        {
            var playlist = await _unitOfWork.Playlists
                .GetFirstAsync(new LoadPlaylistForFavoritingSpecification(command.PlaylistId, command.UserId), cancellationToken);

            if (playlist == null)
                return Result<bool>.NotFound<Playlist>();

            var isFavoriting = command.IsFavoriting
                ? Favorite(playlist, command.UserId, cancellationToken)
                : Unfavorite(playlist, command.UserId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(isFavoriting);
        }
        
        private bool Favorite(Playlist target, long userId, CancellationToken cancellationToken = default)
        {
            var favoritePlaylist = target.FavoritePlaylists.FirstOrDefault(f => f.UserId == userId);

            if (favoritePlaylist is null)
            {
                target.FavoritePlaylists.Add(new FavoritePlaylist
                {
                    Favorited = _dateTimeProvider.Now,
                    UserId = userId,
                    PlaylistId = target.Id
                });
                // await _unitOfWork.FavoritePlaylists.AddAsync(new FavoritePlaylist
                // {
                //     UserId = userId,
                //     PlaylistId = target.Id
                // }, cancellationToken);
            }

            return true;
        }
        
        private bool Unfavorite(Playlist target, long userId, CancellationToken cancellationToken = default)
        {
            var favoritePlaylist = target.FavoritePlaylists.FirstOrDefault(f => f.UserId == userId);

            if (favoritePlaylist is not null)
            {
                target.FavoritePlaylists.Remove(favoritePlaylist);
                _unitOfWork.Playlists.Update(target);
            }

            return false;
        }
    }
}