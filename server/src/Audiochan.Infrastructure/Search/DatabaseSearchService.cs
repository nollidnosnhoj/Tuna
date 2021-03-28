using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Features.Search.SearchAudios;
using Audiochan.Core.Features.Search.SearchUsers;
using Audiochan.Core.Features.Users;
using Audiochan.Core.Features.Users.GetUser;
using Audiochan.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Infrastructure.Search
{
    public class DatabaseSearchService : ISearchService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly AudiochanOptions _audiochanOptions;

        public DatabaseSearchService(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IOptions<AudiochanOptions> options)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _audiochanOptions = options.Value;
        }

        public async Task<PagedList<AudioViewModel>> SearchAudios(SearchAudiosQuery query,
            CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();

            var queryable = _dbContext.Audios
                .DefaultListQueryable(currentUserId)
                .FilterByTags(query.Tags, ",");

            if (!string.IsNullOrWhiteSpace(query.Q))
                queryable = queryable.Where(a => EF.Functions
                    .ILike(a.Title, $"%{query.Q.Trim()}%"));

            var result = await queryable
                .Sort(query.Sort)
                .ProjectToList(_audiochanOptions)
                .PaginateAsync(query, cancellationToken);

            return result;
        }

        public async Task<PagedList<UserViewModel>> SearchUsers(SearchUsersQuery query,
            CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();
            return await _dbContext.Users
                .Where(u => EF.Functions.ILike(u.UserName, $"%{query.Q}%"))
                .ProjectToUser(currentUserId)
                .PaginateAsync(query, cancellationToken);
        }
    }
}