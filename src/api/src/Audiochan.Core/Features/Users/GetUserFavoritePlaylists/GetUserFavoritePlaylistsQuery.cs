using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Playlists;
using Audiochan.Core.Features.Playlists.GetPlaylistDetail;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.GetUserFavoritePlaylists
{
    public record GetUserFavoritePlaylistsQuery : IHasOffsetPage, IRequest<OffsetPagedListDto<PlaylistViewModel>>
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
    
    public class GetUserFavoritePlaylistsQueryHandler : IRequestHandler<GetUserFavoritePlaylistsQuery, OffsetPagedListDto<PlaylistViewModel>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly long _currentUserId;

        public GetUserFavoritePlaylistsQueryHandler(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<OffsetPagedListDto<PlaylistViewModel>> Handle(GetUserFavoritePlaylistsQuery request, 
            CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.UserName == request.Username)
                .SelectMany(u => u.FavoritePlaylists)
                .Where(p => p.Visibility == Visibility.Public)
                .Select(PlaylistMaps.PlaylistToDetailFunc)
                .OffsetPaginateAsync(request, cancellationToken);
        }
    }
}