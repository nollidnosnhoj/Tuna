using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Favorites.Audios.GetFavoriteAudios
{
    public record GetFavoriteAudiosQuery : PaginationQueryRequest<AudioViewModel>
    {
        public string Username { get; init; }
    }

    public class GetFavoriteAudiosQueryHandler : IRequestHandler<GetFavoriteAudiosQuery, PagedList<AudioViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetFavoriteAudiosQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<PagedList<AudioViewModel>> Handle(GetFavoriteAudiosQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            return await _dbContext.FavoriteAudios
                .AsNoTracking()
                .Include(fa => fa.User)
                .Include(fa => fa.Audio)
                .ThenInclude(a => a.User)
                .Include(fa => fa.Audio)
                .ThenInclude(a => a.Favorited)
                .Where(fa => fa.User.UserName == request.Username.Trim().ToLower())
                .OrderByDescending(fa => fa.Created)
                .Select(fa => fa.Audio)
                .ProjectTo<AudioViewModel>(_mapper.ConfigurationProvider, new {currentUserId})
                .PaginateAsync(request, cancellationToken);
        }
    }
}