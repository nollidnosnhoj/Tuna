using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.CQRS;
using Audiochan.Common.Dtos;
using Audiochan.Common.Extensions;
using Audiochan.Common.Interfaces;
using Audiochan.Core.Features.Audios.Dtos;
using Audiochan.Core.Features.Audios.Mappings;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.Queries.GetUserFavoriteAudios
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
                .OffsetPaginate(query.Offset, query.Size)
                .ToListAsync(cancellationToken);
            return new OffsetPagedListDto<AudioDto>(results, query.Offset, query.Size);
        }
    }
}