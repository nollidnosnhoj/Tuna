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
    public record SearchAudiosQuery : IHasPage, IRequest<PagedListDto<AudioViewModel>>
    {
        public string Q { get; init; } = string.Empty;
        public string Tags { get; init; } = string.Empty;
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class SearchAudiosQueryHandler : IRequestHandler<SearchAudiosQuery, PagedListDto<AudioViewModel>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly long _currentUserId;

        public SearchAudiosQueryHandler(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<PagedListDto<AudioViewModel>> Handle(SearchAudiosQuery query,
            CancellationToken cancellationToken)
        {
            var parsedTags = !string.IsNullOrWhiteSpace(query.Tags)
                ? query.Tags.Split(',')
                    .Select(t => t.Trim().ToLower())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToList()
                : new List<string>();

            var queryable = _dbContext.Audios
                .AsNoTracking()
                .Where(a => a.UserId == _currentUserId || a.Visibility == Visibility.Public);

            if (!string.IsNullOrWhiteSpace(query.Q))
                queryable = queryable.Where(a => 
                    EF.Functions.Like(a.Title.ToLower(), $"%{query.Q.ToLower()}%"));

            if (parsedTags.Count > 0)
                queryable = queryable.Where(a => a.Tags.Any(x => parsedTags.Contains(x.Name)));

            return await queryable
                .Select(AudioMaps.AudioToView(_currentUserId))
                .PaginateAsync(query, cancellationToken);
        }
    }
}