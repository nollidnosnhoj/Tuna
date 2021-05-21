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
using Audiochan.Core.Features.Audios.SearchAudios;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Infrastructure.Search
{
    public class DatabaseSearchService : ISearchService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly MediaStorageSettings _storageSettings;

        public DatabaseSearchService(IApplicationDbContext dbContext, IOptions<MediaStorageSettings> options)
        {
            _dbContext = dbContext;
            _storageSettings = options.Value;
        }

        public async Task<PagedList<AudioViewModel>> SearchAudios(SearchAudiosRequest request, CancellationToken cancellationToken = default)
        {
            var queryable = _dbContext.Audios
                .AsNoTracking()
                .Include(x => x.Tags)
                .Include(x => x.User)
                .FilterByTags(request.Tags)
                .ExcludePrivateAudios();
            
            if (!string.IsNullOrWhiteSpace(request.Q))
                queryable = queryable.Where(a => EF.Functions
                    .ILike(a.Title, $"%{request.Q.Trim()}%"));

            var result = await queryable
                .ProjectToList(_storageSettings)
                .PaginateAsync(request.Page, request.Size, cancellationToken);

            return result;
        }
    }
}