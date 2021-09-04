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
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class PlaylistRepository : EfRepository<Playlist>, IPlaylistRepository
    {
        public PlaylistRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<List<PlaylistDto>> GetUserFavoritePlaylists(GetUserFavoritePlaylistsQuery query, CancellationToken ct = default)
        {
            return await DbContext.FavoritePlaylists
                .AsNoTracking()
                .Where(fp => fp.User.UserName == query.Username)
                .OrderByDescending(fp => fp.Favorited)
                .Select(fp => fp.Playlist)
                .ProjectTo<PlaylistDto>(Mapper.ConfigurationProvider)
                .OffsetPaginateAsync(query.Offset, query.Size, ct);
        }
    }
}