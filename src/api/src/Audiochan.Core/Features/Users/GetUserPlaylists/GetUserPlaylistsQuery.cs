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

namespace Audiochan.Core.Features.Users.GetUserPlaylists
{
    public record GetUserPlaylistsQuery : IHasOffsetPage, IRequest<OffsetPagedListDto<PlaylistViewModel>>
    {
        public string Username { get; }
        public int Offset { get; init; } = 1;
        public int Size { get; init; } = 30;

        public GetUserPlaylistsQuery(string username, IHasOffsetPage pageParams)
        {
            Username = username;
            Offset = pageParams.Offset;
            Size = pageParams.Size;
        }
    }
    
    public class GetUserPlaylistsQueryHandler : IRequestHandler<GetUserPlaylistsQuery, OffsetPagedListDto<PlaylistViewModel>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly long _currentUserId;

        public GetUserPlaylistsQueryHandler(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<OffsetPagedListDto<PlaylistViewModel>> Handle(GetUserPlaylistsQuery request, 
            CancellationToken cancellationToken)
        {
            var playlists = await _dbContext.Playlists
                .AsNoTracking()
                .Where(p => p.User.UserName == request.Username)
                .Where(p => p.UserId == _currentUserId || p.Visibility == Visibility.Public)
                .Select(PlaylistMaps.PlaylistToDetailFunc)
                .OffsetPaginateAsync(request, cancellationToken);

            return playlists;
        }
    }
}