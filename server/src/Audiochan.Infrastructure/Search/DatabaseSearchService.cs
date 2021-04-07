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
        private readonly MediaStorageSettings _storageSettings;

        public DatabaseSearchService(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IOptions<MediaStorageSettings> options)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _storageSettings = options.Value;
        }

        public async Task<PagedList<AudioViewModel>> SearchAudios(SearchAudiosRequest request,
            CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();

            var queryable = _dbContext.Audios
                .DefaultListQueryable(currentUserId)
                .FilterByTags(request.Tags, ",");

            if (!string.IsNullOrWhiteSpace(request.Q))
                queryable = queryable.Where(a => EF.Functions
                    .ILike(a.Title, $"%{request.Q.Trim()}%"));

            var result = await queryable
                .ProjectToList(_storageSettings)
                .PaginateAsync(request, cancellationToken);

            return result;
        }

        public async Task<PagedList<UserViewModel>> SearchUsers(SearchUsersRequest request,
            CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();
            return await _dbContext.Users
                .Where(u => EF.Functions.ILike(u.UserName, $"%{request.Q}%"))
                .ProjectToUser(currentUserId)
                .PaginateAsync(request, cancellationToken);
        }
    }
}