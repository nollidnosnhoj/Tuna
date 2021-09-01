using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.SearchAudios
{
    public record SearchAudiosQuery : IHasPage, IRequest<PagedListDto<AudioDto>>
    {
        public string Q { get; init; } = string.Empty;
        public string Tags { get; init; } = string.Empty;
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class SearchAudiosQueryHandler : IRequestHandler<SearchAudiosQuery, PagedListDto<AudioDto>>
    {
        private readonly ISearchService _searchService;

        public SearchAudiosQueryHandler(ISearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<PagedListDto<AudioDto>> Handle(SearchAudiosQuery query,
            CancellationToken cancellationToken)
        {
            return await _searchService.SearchAudiosAsync(query, cancellationToken);
        }
    }
}