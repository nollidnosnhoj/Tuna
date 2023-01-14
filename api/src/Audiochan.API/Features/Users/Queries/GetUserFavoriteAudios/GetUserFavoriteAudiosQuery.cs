using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Audios.Mappings;
using Audiochan.Core.CQRS;
using Audiochan.Core.Dtos;
using Audiochan.Core.Dtos.Wrappers;
using Audiochan.Core.Extensions;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Users.Queries
{
    public record GetUserFavoriteAudiosQuery : IHasOffsetPage, IQueryRequest<OffsetPagedListDto<AudioDto>>
    {
        public string? Username { get; set; }
        public int Offset { get; init; }
        public int Size { get; init; }
    }
    
    public class GetUserFavoriteAudiosQueryHandler : IRequestHandler<GetUserFavoriteAudiosQuery, OffsetPagedListDto<AudioDto>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetUserFavoriteAudiosQueryHandler(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<OffsetPagedListDto<AudioDto>> Handle(GetUserFavoriteAudiosQuery query, CancellationToken cancellationToken)
        {
            _currentUserService.User.TryGetUserId(out var userId);
            var results = await _dbContext.Users
                .Where(u => u.UserName == query.Username)
                .SelectMany(u => u.FavoriteAudios)
                .OrderByDescending(u => u.Favorited)
                .Select(fa => fa.Audio)
                .Project(userId)
                .OffsetPaginateAsync(query.Offset, query.Size, cancellationToken);
            return new OffsetPagedListDto<AudioDto>(results, query.Offset, query.Size);
        }
    }
}