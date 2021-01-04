using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Favorites
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IAudiochanContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public FavoriteService(IAudiochanContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<IResult<List<AudioListViewModel>>> GetUserFavorites(string username, PaginationQuery query, 
            CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();

            if (!await _dbContext.Users.AsNoTracking()
                .AnyAsync(u => u.UserName == username.ToLower(), cancellationToken))
            {
                return Result<List<AudioListViewModel>>.Fail(ResultErrorCode.NotFound, "User was not found.");
            }

            var favorites = await _dbContext.FavoriteAudios
                .AsNoTracking()
                .Include(fa => fa.User)
                .Include(fa => fa.Audio)
                .ThenInclude(a => a.User)
                .Include(fa => fa.Audio)
                .ThenInclude(a => a.Favorited)
                .Where(fa => fa.User.UserName == username.ToLower())
                .OrderByDescending(fa => fa.Created)
                .Select(fa => fa.Audio)
                .Select(MapProjections.AudioList(currentUserId))
                .Paginate(query, cancellationToken);

            return Result<List<AudioListViewModel>>.Success(favorites);
        }

        public async Task<IResult> FavoriteAudio(long userId, string audioId, 
            CancellationToken cancellationToken = default)
        {
            if (!await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Id == userId, cancellationToken))
                return Result.Fail(ResultErrorCode.NotFound, "User was not found.");

            var audio = await _dbContext.Audios
                .AsNoTracking()
                .Select(a => new {a.Id, a.UserId})
                .SingleOrDefaultAsync(a => a.Id == audioId, cancellationToken);

            if (audio == null) 
                return Result.Fail(ResultErrorCode.NotFound, "Audio was not found.");

            // User cannot favorite their own audio
            if (audio.UserId == userId)
                return Result.Fail(ResultErrorCode.Forbidden);
            
            var favorite =
                await _dbContext.FavoriteAudios
                    .SingleOrDefaultAsync(fa => fa.AudioId == audioId && fa.UserId == userId,
                        cancellationToken);

            if (favorite == null)
            {
                favorite = new FavoriteAudio {UserId = userId, AudioId = audioId};
                await _dbContext.FavoriteAudios.AddAsync(favorite, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return Result.Success();
        }

        public async Task<IResult> UnfavoriteAudio(long userId, string audioId, 
            CancellationToken cancellationToken = default)
        {
            if (!await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Id == userId, cancellationToken))
                return Result.Fail(ResultErrorCode.NotFound, "User was not found.");

            if (!await _dbContext.Audios.AsNoTracking().AnyAsync(a => a.Id == audioId, cancellationToken))
                return Result.Fail(ResultErrorCode.NotFound, "Audio was not found.");

            var favorite =
                await _dbContext.FavoriteAudios
                    .SingleOrDefaultAsync(fa => fa.AudioId == audioId && fa.UserId == userId,
                        cancellationToken);

            if (favorite != null)
            {
                _dbContext.FavoriteAudios.Remove(favorite);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return Result.Success();
        }

        public async Task<bool> CheckIfUserFavorited(long userId, string audioId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.FavoriteAudios.AsNoTracking()
                .AnyAsync(fa => fa.AudioId == audioId && fa.UserId == userId, cancellationToken);
        }
    }
}