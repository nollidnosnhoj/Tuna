using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.SearchAudios;

namespace Audiochan.Core.Interfaces
{
    public interface ISearchService
    {
        Task<PagedListDto<AudioDto>> SearchAudiosAsync(SearchAudiosQuery query, CancellationToken ct = default);
    }
}