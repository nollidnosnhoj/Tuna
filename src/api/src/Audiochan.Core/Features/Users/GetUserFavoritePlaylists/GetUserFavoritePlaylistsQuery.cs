using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Playlists;
using Audiochan.Core.Features.Playlists.GetPlaylistDetail;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.GetUserFavoritePlaylists
{
    public record GetUserFavoritePlaylistsQuery : IHasPage, IRequest<PagedListDto<PlaylistViewModel>>
    {
        public string Username { get; }
        public int Page { get; init; } = 1;
        public int Size { get; init; } = 30;

        public GetUserFavoritePlaylistsQuery(string username, IHasPage pageParams)
        {
            Username = username;
            Page = pageParams.Page;
            Size = pageParams.Size;
        }
    }
    
    public class GetUserFavoritePlaylistsQueryHandler : IRequestHandler<GetUserFavoritePlaylistsQuery, PagedListDto<PlaylistViewModel>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _currentUserId;

        public GetUserFavoritePlaylistsQueryHandler(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<PagedListDto<PlaylistViewModel>> Handle(GetUserFavoritePlaylistsQuery request, 
            CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.FavoritePlaylists)
                .ThenInclude(fa => fa.Playlist)
                .Where(u => u.UserName == request.Username)
                .SelectMany(u => u.FavoritePlaylists)
                .Select(fa => fa.Playlist)
                .FilterVisibility(_currentUserId, FilterVisibilityMode.OnlyPublic)
                .Select(PlaylistMaps.PlaylistToDetailFunc)
                .PaginateAsync(request, cancellationToken);
        }
    }
}