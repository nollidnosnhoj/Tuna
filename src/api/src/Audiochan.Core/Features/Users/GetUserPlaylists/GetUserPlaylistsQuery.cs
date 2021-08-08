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

namespace Audiochan.Core.Features.Users.GetUserPlaylists
{
    public record GetUserPlaylistsQuery : IHasPage, IRequest<PagedListDto<PlaylistViewModel>>
    {
        public string Username { get; }
        public int Page { get; init; } = 1;
        public int Size { get; init; } = 30;

        public GetUserPlaylistsQuery(string username, IHasPage pageParams)
        {
            Username = username;
            Page = pageParams.Page;
            Size = pageParams.Size;
        }
    }
    
    public class GetUserPlaylistsQueryHandler : IRequestHandler<GetUserPlaylistsQuery, PagedListDto<PlaylistViewModel>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly long _currentUserId;

        public GetUserPlaylistsQueryHandler(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<PagedListDto<PlaylistViewModel>> Handle(GetUserPlaylistsQuery request, 
            CancellationToken cancellationToken)
        {
            var playlists = await _dbContext.Playlists
                .Include(p => p.Tags)
                .Include(p => p.User)
                .Where(p => p.User.UserName == request.Username)
                .FilterVisibility(_currentUserId, FilterVisibilityMode.OnlyPublic)
                .Select(PlaylistMaps.PlaylistToDetailFunc)
                .PaginateAsync(request, cancellationToken);

            return playlists;
        }
    }
}