using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Playlists;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Users.GetUserPlaylists
{
    public record GetUserPlaylistsQuery(string Username) : IHasOffsetPage, IRequest<OffsetPagedListDto<PlaylistDto>>
    {
        public int Offset { get; init; } = 1;
        public int Size { get; init; } = 30;
    }

    public sealed class GetUserPlaylistsSpecification : Specification<Playlist>
    {
        public GetUserPlaylistsSpecification(string username)
        {
            Query.AsNoTracking();
            Query.Where(p => p.User.UserName == username);
            Query.OrderBy(p => p.Title);
        }
    }

    public class GetUserPlaylistsQueryHandler : IRequestHandler<GetUserPlaylistsQuery, OffsetPagedListDto<PlaylistDto>>
    {
        private readonly IUnitOfWork _dbContext;

        public GetUserPlaylistsQueryHandler(IUnitOfWork dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OffsetPagedListDto<PlaylistDto>> Handle(GetUserPlaylistsQuery request, 
            CancellationToken cancellationToken)
        {
            var spec = new GetUserPlaylistsSpecification(request.Username);
            var playlists = await _dbContext.Playlists
                .GetOffsetPagedListAsync<PlaylistDto>(spec, request.Offset, request.Size, cancellationToken);

            return new OffsetPagedListDto<PlaylistDto>(playlists, request.Offset, request.Size);
        }
    }
}