using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.SearchAudios;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;

namespace Audiochan.Infrastructure.Search
{
    public class PostgresSearchService : ISearchService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostgresSearchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedListDto<AudioDto>> SearchAudiosAsync(SearchAudiosQuery query, CancellationToken ct = default)
        {
            var spec = new SearchAudiosSpecification(query);
            var count = await _unitOfWork.Audios.CountAsync(spec, ct);
            var results = await _unitOfWork.Audios.GetPagedListAsync<AudioDto>(spec, query.Page, query.Size, ct);
            return new PagedListDto<AudioDto>(results, count, query.Page, query.Size);
        }
    }
}