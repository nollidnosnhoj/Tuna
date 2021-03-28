using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Features.Search.SearchAudios;
using Audiochan.Core.Features.Search.SearchUsers;
using Audiochan.Core.Features.Users.GetUser;

namespace Audiochan.Core.Interfaces
{
    public interface ISearchService
    {
        Task<PagedList<AudioViewModel>> SearchAudios(SearchAudiosQuery query,
            CancellationToken cancellationToken = default);

        Task<PagedList<UserViewModel>> SearchUsers(SearchUsersQuery query,
            CancellationToken cancellationToken = default);
    }
}