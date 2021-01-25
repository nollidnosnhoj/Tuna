using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.Models;
using Microsoft.AspNetCore.Http;

namespace Audiochan.Core.Interfaces
{
    public interface IAudioService
    {
        Task<PagedList<AudioListViewModel>> GetFeed(string userId, PaginationQuery query, 
            CancellationToken cancellationToken = default);
        Task<PagedList<AudioListViewModel>> GetList(GetAudioListQuery query, CancellationToken cancellationToken = default);
        Task<IResult<AudioDetailViewModel>> Get(string audioId, CancellationToken cancellationToken = default);
        Task<IResult<AudioDetailViewModel>> GetRandom(CancellationToken cancellationToken = default);
        Task<IResult<AudioDetailViewModel>> Create(UploadAudioRequest request, 
            CancellationToken cancellationToken = default);
        Task<IResult<AudioDetailViewModel>> Update(string audioId, UpdateAudioRequest request, 
            CancellationToken cancellationToken = default);
        Task<IResult> Remove(string id, CancellationToken cancellationToken = default);
        Task<IResult<string>> AddPicture(string audioId, IFormFile file,
            CancellationToken cancellationToken = default);
        Task<PagedList<PopularTagViewModel>> GetPopularTags(PaginationQuery paginationQuery,
            CancellationToken cancellationToken = default);
    }
}