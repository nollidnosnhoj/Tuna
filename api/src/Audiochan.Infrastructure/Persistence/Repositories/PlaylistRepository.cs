using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetLatestAudios;
using Audiochan.Core.Features.Playlists;
using Audiochan.Core.Features.Playlists.GetPlaylistAudios;
using Audiochan.Core.Features.Playlists.GetPlaylistDetail;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class PlaylistRepository : EfRepository<Playlist>, IPlaylistRepository
    {
        public PlaylistRepository(ApplicationDbContext dbContext, ICurrentUserService currentUserService) 
            : base(dbContext, currentUserService)
        {
        }

        public async Task<PlaylistDetailViewModel?> Get(Guid id,
            CancellationToken cancellationToken = default)
        {
            return await GetQueryable
                .Where(x => x.Id == id)
                .Select(PlaylistMaps.PlaylistToDetailFunc)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<PagedListDto<AudioViewModel>> GetAudios(GetPlaylistAudiosQuery query, 
            CancellationToken cancellationToken = default)
        {
            var currentUserId = CurrentUserService.GetUserId();
            
            return await DbSet
                .Include(p => p.Audios)
                .Where(p => p.Id == query.Id)
                .Where(p => p.UserId == currentUserId || p.Visibility != Visibility.Private)
                .SelectMany(p => p.Audios)
                .Select(pa => pa.Audio)
                .Where(a => a.UserId == currentUserId || a.Visibility == Visibility.Public)
                .Select(AudioMaps.AudioToView)
                .PaginateAsync(query, cancellationToken);
        }

        public async Task<Playlist?> LoadWithAudios(Guid id, List<Guid>? audioIds = null, 
            CancellationToken cancellationToken = default)
        {
            IQueryable<Playlist> queryable = DbSet;
            if (audioIds is null || audioIds.Count == 0)
            {
                queryable = queryable.Include(x => x.Audios);
            }
            else
            {
                queryable = queryable
                    .Include(x => x.Audios.Where(a => audioIds.Contains(a.AudioId)));
            }

            return await queryable.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Playlist?> LoadForUpdating(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(p => p.User)
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Playlist?> LoadWithFavorites(Guid id, string userId = "", CancellationToken cancellationToken = default)
        {
            var queryable = DbSet
                .IgnoreQueryFilters()
                .Where(a => a.Id == id);

            queryable = !string.IsNullOrEmpty(userId) 
                ? queryable.Include(a => 
                    a.Favorited.Where(fa => fa.UserId == userId)) 
                : queryable.Include(a => a.Favorited);

            return await queryable.SingleOrDefaultAsync(cancellationToken);
        }
        
        private IQueryable<Playlist> GetQueryable => DbSet
            .AsNoTracking()
            .Include(x => x.Tags)
            .Include(x => x.User);
    }
}