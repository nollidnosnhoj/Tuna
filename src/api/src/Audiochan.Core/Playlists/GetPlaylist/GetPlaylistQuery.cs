using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Playlists.GetPlaylist
{
    public record GetPlaylistQuery(long Id) : IRequest<PlaylistDto?>;

    public sealed class GetPlaylistByIdSpecification : Specification<Playlist>
    {
        public GetPlaylistByIdSpecification(long id)
        {
            Query.AsNoTracking();
            Query.Where(p => p.Id == id);
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
                .GetFirstAsync<PlaylistDto>(new GetPlaylistByIdSpecification(request.Id), cancellationToken);
        }
    }
}