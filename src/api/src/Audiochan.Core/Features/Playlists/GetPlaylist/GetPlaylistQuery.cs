using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Playlists.GetPlaylist
{
    public record GetPlaylistQuery(long Id) : IRequest<PlaylistDto?>;
    
    public class GetPlaylistQueryHandler : IRequestHandler<GetPlaylistQuery, PlaylistDto?>
    {
        private readonly long _currentUserId;
        private readonly ApplicationDbContext _unitOfWork;

        public GetPlaylistQueryHandler(ICurrentUserService currentUserService, ApplicationDbContext unitOfWork)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
        }

        public async Task<PlaylistDto?> Handle(GetPlaylistQuery request, CancellationToken cancellationToken)
        {
            var playlist = await _unitOfWork.Playlists
                .AsNoTracking()
                .Where(x => x.Id == request.Id)
                .Select(PlaylistMaps.PlaylistToDetailFunc)
                .SingleOrDefaultAsync(cancellationToken);

            return playlist;
        }
    }
}