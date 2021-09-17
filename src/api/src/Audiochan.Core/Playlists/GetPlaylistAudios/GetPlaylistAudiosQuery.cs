using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Audios;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models.Pagination;
using MediatR;

namespace Audiochan.Core.Playlists.GetPlaylistAudios
{
    public record GetPlaylistAudiosQuery(long Id) : IHasCursorPage<long>, IRequest<CursorPagedListDto<PlaylistAudioDto, long>>
    {
        public long Cursor { get; init; }
        public int Size { get; init; }
    }

    public class GetPlaylistAudiosQueryHandler : IRequestHandler<GetPlaylistAudiosQuery, CursorPagedListDto<PlaylistAudioDto, long>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly long _currentUserId;

        public GetPlaylistAudiosQueryHandler(IUnitOfWork unitOfWork, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _currentUserId = authService.GetUserId();
        }

        public async Task<CursorPagedListDto<PlaylistAudioDto, long>> Handle(GetPlaylistAudiosQuery request, CancellationToken cancellationToken)
        {
            var results = await _unitOfWork.Audios.GetPlaylistAudios(request, cancellationToken);
            return new CursorPagedListDto<PlaylistAudioDto, long>(results, request.Size);
        }
    }
}