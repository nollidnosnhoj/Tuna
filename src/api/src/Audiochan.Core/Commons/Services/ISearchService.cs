using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Commons.Dtos.Wrappers;
using Audiochan.Core.Features.Audios.Models;

namespace Audiochan.Core.Commons.Services
{
    public interface ISearchService
    {
        /// <summary>
        /// Search for audios based on the desired requirements.
        /// </summary>
        /// <param name="filter">An object containing requirements for searching.</param>
        /// <param name="ct"></param>
        /// <returns>A paginated list of audios</returns>
        Task<PagedListDto<AudioDto>> SearchAudiosAsync(SearchAudioFilter filter, CancellationToken ct = default);
    }
}