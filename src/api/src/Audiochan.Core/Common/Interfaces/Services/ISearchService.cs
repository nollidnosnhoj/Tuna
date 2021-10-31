using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Audios;
using Audiochan.Core.Common.Models.Pagination;

namespace Audiochan.Core.Common.Interfaces.Services
{
    public interface ISearchService
    {
        /// <summary>
        /// Search for audios based on the desired requirements.
        /// </summary>
        /// <param name="query">An object containing requirements for searching.</param>
        /// <param name="ct"></param>
        /// <returns>A paginated list of audios</returns>
        Task<PagedListDto<AudioDto>> SearchAudiosAsync(SearchAudiosQuery query, CancellationToken ct = default);
    }
}