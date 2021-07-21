using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Features.Audios.SearchAudios;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Persistence.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class AudioRepository : EfRepository<Audio>, IAudioRepository
    {
        public AudioRepository(ApplicationDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper) 
            : base(dbContext, currentUserService, mapper)
        {
        }

        public async Task<AudioDetailViewModel?> GetAudio(Guid id, CancellationToken cancellationToken = default)
        {
            var currentUserId = CurrentUserService.GetUserId();
            return await DbSet
                .AsNoTracking()
                .Include(x => x.Tags)
                .Include(x => x.User)
                .Where(x => x.Id == id)
                .ProjectTo<AudioDetailViewModel>(Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<Audio?> LoadForUpdating(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(a => a.User)
                .Include(a => a.Tags)
                .SingleOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<Audio?> LoadWithFavorites(Guid id, string userId = "", CancellationToken cancellationToken = default)
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

        public async Task<List<AudioViewModel>> GetLatestAudios(GetLatestAudioQuery query, 
            CancellationToken cancellationToken = default)
        {
            var queryable = DbSet
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.Visibility == Visibility.Public);

            if (query.Cursor is not null)
            {
                var (id, since) = CursorHelpers.Decode(query.Cursor);
                if (id is not null && since is not null)
                {
                    queryable = queryable
                        .Where(a => a.Created < since || a.Created == since && a.Id.CompareTo(id) < 0);
                }
            }

            return await queryable
                .ProjectTo<AudioViewModel>(Mapper.ConfigurationProvider)
                .Take(query.Size)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedListDto<AudioViewModel>> SearchAudios(SearchAudiosQuery query, CancellationToken cancellationToken = default)
        {
            var parsedTags = !string.IsNullOrWhiteSpace(query.Tags)
                ? query.Tags.Split(',')
                    .Select(t => t.Trim().ToLower())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToList()
                : new List<string>();
            
            var queryable = DbSet.AsNoTracking()
                .Include(x => x.Tags)
                .Include(x => x.User)
                .Where(x => x.Visibility == Visibility.Public);

            if (!string.IsNullOrWhiteSpace(query.Q))
                queryable = queryable.Where(a => 
                    EF.Functions.Like(a.Title.ToLower(), $"%{query.Q.ToLower()}%"));

            if (parsedTags.Count > 0)
                queryable = queryable.Where(a => a.Tags.Any(x => parsedTags.Contains(x.Name)));

            return await queryable
                .ProjectTo<AudioViewModel>(Mapper.ConfigurationProvider)
                .PaginateAsync(query, cancellationToken);
        }

        public async Task<PagedListDto<AudioViewModel>> GetFollowedAudios(List<string> followingIds, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Include(x => x.User)
                .Where(a => a.Visibility == Visibility.Public)
                .Where(a => followingIds.Contains(a.UserId))
                .ProjectTo<AudioViewModel>(Mapper.ConfigurationProvider)
                .OrderByDescending(a => a.Created)
                .PaginateAsync(cancellationToken: cancellationToken);
        }
    }
}