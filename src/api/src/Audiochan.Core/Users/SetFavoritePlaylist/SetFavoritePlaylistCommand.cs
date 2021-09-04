using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Users.SetFavoritePlaylist
{
    public record SetFavoritePlaylistCommand(long PlaylistId, long UserId, bool IsFavoriting) : IRequest<Result>
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

    public class SetFavoritePlaylistCommandHandler : IRequestHandler<SetFavoritePlaylistCommand, Result>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUnitOfWork _unitOfWork;

        public SetFavoritePlaylistCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
        {
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result> Handle(SetFavoritePlaylistCommand command, CancellationToken cancellationToken)
        {
            var playlist = await _unitOfWork.Playlists
                .GetFirstAsync(new LoadPlaylistForFavoritingSpecification(command.PlaylistId, command.UserId), cancellationToken);

            if (playlist == null)
                return Result.NotFound<Playlist>();

            if (command.IsFavoriting)
            {
                playlist.Favorite(command.UserId, _dateTimeProvider.Now);
            }
            else
            {
                playlist.UnFavorite(command.UserId);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}