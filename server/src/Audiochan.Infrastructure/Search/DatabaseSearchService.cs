using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Extensions;
using Audiochan.Core.Extensions.MappingExtensions;
using Audiochan.Core.Extensions.QueryableExtensions;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Features.Search.SearchAudios;
using Audiochan.Core.Features.Search.SearchUsers;
using Audiochan.Core.Features.Users;
using Audiochan.Core.Features.Users.GetUser;
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

        public async Task<PagedList<AudioViewModel>> SearchAudios(SearchAudiosRequest request,
            CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();
            
            var parsedTags = request.Tags.Split(',')
                .Select(t => t.Trim().ToLower())
                .ToArray();

            var queryable = _dbContext.Audios
                .BaseListQueryable(currentUserId)
                .FilterByTags(parsedTags);

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