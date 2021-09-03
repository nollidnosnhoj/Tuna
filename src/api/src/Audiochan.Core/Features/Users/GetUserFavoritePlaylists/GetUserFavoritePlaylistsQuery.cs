using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Features.Playlists;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using MediatR;

namespace Audiochan.Core.Features.Users.GetUserFavoritePlaylists
{
    public record GetUserFavoritePlaylistsQuery : IHasOffsetPage, IRequest<OffsetPagedListDto<PlaylistDto>>
    {
        public string Username { get; }
        public int Offset { get; init; } = 1;
        public int Size { get; init; } = 30;

        public GetUserFavoritePlaylistsQuery(string username, IHasOffsetPage pageParams)
        {
            Username = username;
            Offset = pageParams.Offset;
            Size = pageParams.Size;
        }
    }
    
    public class GetUserFavoritePlaylistsQueryHandler : IRequestHandler<GetUserFavoritePlaylistsQuery, OffsetPagedListDto<PlaylistDto>>
    {
        private readonly IUnitOfWork _dbContext;
        private readonly long _currentUserId;

        public GetUserFavoritePlaylistsQueryHandler(IUnitOfWork dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<OffsetPagedListDto<PlaylistDto>> Handle(GetUserFavoritePlaylistsQuery request, 
            CancellationToken cancellationToken)
        {
            var results = await _dbContext.Playlists.GetUserFavoritePlaylists(request, cancellationToken);
            return new OffsetPagedListDto<PlaylistDto>(results, request.Offset, request.Size);
        }
    }
}