using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.FavoritePlaylists.CheckIfPlaylistFavorited
{
    public record CheckIfPlaylistFavoritedQuery(Guid PlaylistId, string UserId) : IRequest<bool>
    {
        
    }

    public class CheckIfPlaylistFavoritedQueryHandler : IRequestHandler<CheckIfPlaylistFavoritedQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckIfPlaylistFavoritedQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CheckIfPlaylistFavoritedQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.FavoritePlaylists
                .ExistsAsync(fp => fp.PlaylistId == request.PlaylistId 
                                   && fp.UserId == request.UserId, cancellationToken);
        }
    }
}