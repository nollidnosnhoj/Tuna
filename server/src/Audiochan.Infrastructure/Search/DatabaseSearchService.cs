using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Features.Search;
using Audiochan.Core.Features.Users.GetUser;
using Audiochan.Core.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Search
{
    public class DatabaseSearchService : ISearchService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public DatabaseSearchService(IApplicationDbContext dbContext, ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<PagedList<AudioViewModel>> SearchAudios(SearchAudiosQuery query,
            CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();

            var queryable = _dbContext.Audios
                .DefaultQueryable(currentUserId)
                .FilterByGenre(query.Genre)
                .FilterByTags(query.Tags, ",");

            if (!string.IsNullOrWhiteSpace(query.Q))
                queryable = queryable.Where(a => EF.Functions
                    .ILike(a.Title, $"%{query.Q.Trim()}%"));

            var result = await queryable
                .Sort(query.Sort)
                .ProjectTo<AudioViewModel>(_mapper.ConfigurationProvider, new {currentUserId})
                .PaginateAsync(query, cancellationToken);

            return result;
        }

        public async Task<PagedList<UserViewModel>> SearchUsers(SearchUsersQuery query,
            CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();
            return await _dbContext.Users
                .Where(u => EF.Functions.ILike(u.UserName, $"%{query.Q}%"))
                .ProjectTo<UserViewModel>(_mapper.ConfigurationProvider, new {currentUserId})
                .PaginateAsync(query, cancellationToken);
        }
    }
}