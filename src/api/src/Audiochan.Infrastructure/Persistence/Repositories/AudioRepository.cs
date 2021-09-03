﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Playlists.GetPlaylistAudios;
using Audiochan.Core.Features.Users.GetUserFavoriteAudios;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.Infrastructure.Persistence.Repositories.Abstractions;
using Audiochan.Infrastructure.Persistence.Repositories.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class AudioRepository : EfRepository<Audio>, IAudioRepository
    {
        public AudioRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<AudioDto>> GetPlaylistAudios(GetPlaylistAudiosQuery query, CancellationToken cancellationToken = default)
        {
            return await DbContext.Playlists
                .Where(p => p.Id == query.Id)
                .AsNoTracking()
                .SelectMany(p => p.Audios)
                .OrderByDescending(a => a.Id)
                .Select(AudioMaps.AudioToView())
                .CursorPaginateAsync(query.Cursor, query.Size, cancellationToken);
        }

        public async Task<List<AudioDto>> GetUserFavoriteAudios(GetUserFavoriteAudiosQuery query, CancellationToken cancellationToken = default)
        {
            return await DbContext.FavoriteAudios
                .Where(fa => fa.User.UserName == query.Username)
                .OrderByDescending(fa => fa.Favorited)
                .AsNoTracking()
                .Select(fa => fa.Audio)
                .Select(AudioMaps.AudioToView())
                .OffsetPaginateAsync(query.Offset, query.Size, cancellationToken);
        }
    }
}