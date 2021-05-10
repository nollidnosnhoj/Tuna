using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.SearchAudios;

namespace Audiochan.Core.Common.Interfaces
{
    public interface ISearchService
    {
        Task<PagedList<AudioViewModel>> SearchAudios(SearchAudiosRequest request,
            CancellationToken cancellationToken = default);
    }
}