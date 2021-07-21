using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Features.Audios.SearchAudios;

namespace Audiochan.Core.Repositories
{
    public interface IAudioRepository : IGenericRepository<Audio>
    {
        Task<AudioDetailViewModel?> GetAudio(Guid id, CancellationToken cancellationToken = default);

        Task<Audio?> LoadForUpdating(Guid id, CancellationToken cancellationToken = default);

        Task<Audio?> LoadWithFavorites(Guid id, string userId = "", CancellationToken cancellationToken = default);

        Task<List<AudioViewModel>> GetLatestAudios(GetLatestAudioQuery query, 
            CancellationToken cancellationToken = default);

        Task<PagedListDto<AudioViewModel>> SearchAudios(SearchAudiosQuery query, 
            CancellationToken cancellationToken = default);

        Task<PagedListDto<AudioViewModel>> GetFollowedAudios(List<string> followingIds,
            CancellationToken cancellationToken = default);
    }
}