using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.GetAudioList;
using MediatR;

namespace Audiochan.Core.Features.Playlists
{
    public class GetPlaylistAudiosQuery : IHasPage, IRequest<PagedListDto<AudioViewModel>>
    {
        public long PlaylistId { get; init; }
        public string? PrivateKey { get; init; }
        public int Page { get; init; }
        public int Size { get; init; }
    }
    
    public class GetPlaylistAudiosQueryHandler : IRequestHandler<GetPlaylistAudiosQuery, PagedListDto<AudioViewModel>>
    {
        public async Task<PagedListDto<AudioViewModel>> Handle(GetPlaylistAudiosQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}