using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Audios;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Playlists.GetPlaylistAudios;
using Audiochan.Core.Users.GetUserFavoriteAudios;
using Audiochan.Domain.Entities;
using Audiochan.Infrastructure.Persistence.Repositories.Abstractions;
using Audiochan.Infrastructure.Persistence.Repositories.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class AudioRepository : EfRepository<Audio>, IAudioRepository
    {
        public AudioRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<List<PlaylistAudioDto>> GetPlaylistAudios(GetPlaylistAudiosQuery query, CancellationToken cancellationToken = default)
        {
            return await DbContext.PlaylistAudios
                .Where(p => p.PlaylistId == query.Id)
                .AsNoTracking()
                .OrderByDescending(a => a.AddedBy)
                .ProjectTo<PlaylistAudioDto>(Mapper.ConfigurationProvider)
                .CursorPaginateAsync(query.Cursor, query.Size, cancellationToken);
        }

        public async Task<List<AudioDto>> GetUserFavoriteAudios(GetUserFavoriteAudiosQuery query, CancellationToken cancellationToken = default)
        {
            return await DbContext.FavoriteAudios
                .Where(fa => fa.User.UserName == query.Username)
                .OrderByDescending(fa => fa.Favorited)
                .AsNoTracking()
                .Select(fa => fa.Audio)
                .ProjectTo<AudioDto>(Mapper.ConfigurationProvider)
                .OffsetPaginateAsync(query.Offset, query.Size, cancellationToken);
        }
    }
}