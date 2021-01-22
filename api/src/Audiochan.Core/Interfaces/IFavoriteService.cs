using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.Models;

namespace Audiochan.Core.Interfaces
{
    public interface IFavoriteService
    {
        Task<PagedList<AudioListViewModel>> GetUserFavorites(string username, PaginationQuery query,
            CancellationToken cancellationToken = default);
        Task<IResult> FavoriteAudio(long userId, string audioId, CancellationToken cancellationToken = default);
        Task<IResult> UnfavoriteAudio(long userId, string audioId, CancellationToken cancellationToken = default);
        Task<bool> CheckIfUserFavorited(long userId, string audioId, CancellationToken cancellationToken = default);
    }
}