using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.Models;

namespace Audiochan.Core.Interfaces
{
    public interface IAudioService
    {
        Task<List<AudioListViewModel>> GetFeed(long userId, PaginationQuery query, 
            CancellationToken cancellationToken = default);
        Task<List<AudioListViewModel>> GetList(GetAudioListQuery query, CancellationToken cancellationToken = default);
        Task<IResult<AudioDetailViewModel>> Get(string audioId, CancellationToken cancellationToken = default);
        Task<IResult<bool>> AddView(string audioId, string ipAddress, CancellationToken cancellationToken = default);
        Task<string?> GetRandomAudioId(CancellationToken cancellationToken = default);
        Task<IResult<AudioDetailViewModel>> Create(UploadAudioRequest request, 
            CancellationToken cancellationToken = default);
        Task<IResult<AudioDetailViewModel>> Update(string audioId, UpdateAudioRequest request, 
            CancellationToken cancellationToken = default);
        Task<IResult> Remove(string id, CancellationToken cancellationToken = default);
    }
}