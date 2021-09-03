using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Playlists.GetDuplicatedAudiosInPlaylist
{
    public record GetDuplicatedAudiosInPlaylistQuery(long PlaylistId, List<long> AudioIds) : IRequest<List<long>>;

    public sealed class GetDuplicatedAudiosInPlaylistSpecification : Specification<PlaylistAudio, long>
    {
        public GetDuplicatedAudiosInPlaylistSpecification(long playlistId, List<long> audioIds)
        {
            Query.AsNoTracking();
            Query.Where(pa => pa.PlaylistId == playlistId);
            Query.Where(pa => audioIds.Contains(pa.AudioId));
            Query.Select(pa => pa.AudioId);
        }
    }

    public class CheckDuplicatedAudiosQueryHandler 
        : IRequestHandler<GetDuplicatedAudiosInPlaylistQuery, List<long>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckDuplicatedAudiosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<long>> Handle(GetDuplicatedAudiosInPlaylistQuery request, 
            CancellationToken cancellationToken)
        {
            var spec = new GetDuplicatedAudiosInPlaylistSpecification(request.PlaylistId, request.AudioIds);
            return await _unitOfWork.PlaylistAudios.GetListAsync(spec, cancellationToken);
        }
    }
}