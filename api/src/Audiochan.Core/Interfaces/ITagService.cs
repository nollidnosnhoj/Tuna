using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Tags.Models;

namespace Audiochan.Core.Interfaces
{
    public interface ITagService
    {
        Task<IResult<List<PopularTagViewModel>>> GetPopularTags(PaginationQuery paginationQuery, CancellationToken cancellationToken = default);
        Task<List<Tag>> CreateNewTags(IEnumerable<string?> tags, CancellationToken cancellationToken = default);
    }
}