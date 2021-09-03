using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Features.Playlists;
using Audiochan.Core.Features.Users.GetUserFavoritePlaylists;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.Infrastructure.Persistence.Repositories.Abstractions;
using Audiochan.Infrastructure.Persistence.Repositories.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class PlaylistRepository : EfRepository<Playlist>, IPlaylistRepository
    {
        public PlaylistRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<PlaylistDto>> GetUserFavoritePlaylists(GetUserFavoritePlaylistsQuery query, CancellationToken ct = default)
        {
            return await DbContext.FavoritePlaylists
                .AsNoTracking()
                .Where(fp => fp.User.UserName == query.Username)
                .OrderByDescending(fp => fp.Favorited)
                .Select(fp => fp.Playlist)
                .Select(PlaylistMaps.PlaylistToDetailFunc)
                .OffsetPaginateAsync(query.Offset, query.Size, ct);
        }
    }
}