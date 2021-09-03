using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Playlists.GetPlaylist
{
    public record GetPlaylistQuery(long Id) : IRequest<PlaylistDto?>;

    public sealed class GetPlaylistByIdSpecification : Specification<Playlist, PlaylistDto>
    {
        public GetPlaylistByIdSpecification(long id)
        {
            Query.AsNoTracking();
            Query.Where(p => p.Id == id);
            Query.Select(PlaylistMaps.PlaylistToDetailFunc);
        }
    }

    public class GetPlaylistQueryHandler : IRequestHandler<GetPlaylistQuery, PlaylistDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public GetPlaylistQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PlaylistDto?> Handle(GetPlaylistQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Playlists
                .GetFirstAsync(new GetPlaylistByIdSpecification(request.Id), cancellationToken);
        }
    }
}