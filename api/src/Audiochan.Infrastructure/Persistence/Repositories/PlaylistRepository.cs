using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Features.Playlists.GetPlaylistAudios;
using Audiochan.Core.Features.Playlists.GetPlaylistDetail;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Persistence.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class PlaylistRepository : EfRepository<Playlist>, IPlaylistRepository
    {
        public PlaylistRepository(ApplicationDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper) 
            : base(dbContext, currentUserService, mapper)
        {
        }

        public async Task<PlaylistDetailViewModel?> GetPlaylistDetail(Guid id,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.Id == id)
                .ProjectTo<PlaylistDetailViewModel>(Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);
        }

    }
}