﻿using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Features.Playlists;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Users.GetUserPlaylists
{
    public record GetUserPlaylistsQuery(string Username) : IHasOffsetPage, IRequest<OffsetPagedListDto<PlaylistDto>>
    {
        public int Offset { get; init; } = 1;
        public int Size { get; init; } = 30;
    }

    public sealed class GetUserPlaylistsSpecification : Specification<Playlist, PlaylistDto>
    {
        public GetUserPlaylistsSpecification(string username)
        {
            Query.AsNoTracking();
            Query.Where(p => p.User.UserName == username);
            Query.OrderBy(p => p.Title);
            Query.Select(PlaylistMaps.PlaylistToDetailFunc);
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
            var playlists = await _dbContext.Playlists
                .GetOffsetPagedListAsync(new GetUserPlaylistsSpecification(request.Username), request.Offset, request.Size, cancellationToken);

            return new OffsetPagedListDto<PlaylistDto>(playlists, request.Offset, request.Size);
        }
    }
}