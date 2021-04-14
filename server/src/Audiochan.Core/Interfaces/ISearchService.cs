using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Models.Responses;
using Audiochan.Core.Models.ViewModels;

namespace Audiochan.Core.Interfaces
{
    public interface ISearchService
    {
        Task<PagedList<AudioViewModel>> SearchAudios(string searchTerm, string[] filteredTags, int page = 1, int limit = 30,
            CancellationToken cancellationToken = default);
    }
}