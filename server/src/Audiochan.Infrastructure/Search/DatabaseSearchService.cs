using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Extensions;
using Audiochan.Core.Extensions.MappingExtensions;
using Audiochan.Core.Extensions.QueryableExtensions;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models.Responses;
using Audiochan.Core.Models.ViewModels;
using Audiochan.Core.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Infrastructure.Search
{
    public class DatabaseSearchService : ISearchService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly MediaStorageSettings _storageSettings;

        public DatabaseSearchService(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IOptions<MediaStorageSettings> options)
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
                queryable = queryable.FilterByTags(filteredTags);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
                queryable = queryable.Where(a => EF.Functions
                    .ILike(a.Title, $"%{searchTerm.Trim()}%"));

            var result = await queryable
                .ProjectToList(_storageSettings)
                .PaginateAsync(page, limit, cancellationToken);

            return result;
        }
    }
}