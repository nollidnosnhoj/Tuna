using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Extensions.QueryableExtensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Features.Audios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Infrastructure.Search
{
    public class DatabaseSearchService : ISearchService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly MediaStorageSettings _storageSettings;

        public DatabaseSearchService(IApplicationDbContext dbContext, ICurrentUserService currentUserService,
            IOptions<MediaStorageSettings> options)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _storageSettings = options.Value;
        }

        public async Task<PagedList<AudioViewModel>> SearchAudios(string searchTerm, string[] filteredTags,
            int page = 1, int limit = 30, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();

            var queryable = _dbContext.Audios
                .BaseListQueryable(currentUserId);

            if (filteredTags.Length > 0)
            {
                queryable = queryable.Where(a => a.Tags.Any(t => filteredTags.Contains(t.Name)));
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
                queryable = queryable.Where(a => EF.Functions
                    .ILike(a.Title, $"%{searchTerm.Trim()}%"));

            var result = await queryable
                .OrderByDescending(a => a.Created)
                .ProjectToList(_storageSettings)
                .PaginateAsync(page, limit, cancellationToken);

            return result;
        }
    }
}