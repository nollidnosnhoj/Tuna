using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Models.ViewModels;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Features.Search.SearchAudios;
using Audiochan.Core.Features.Search.SearchUsers;
using Audiochan.Core.Features.Users.GetUser;

namespace Audiochan.Core.Interfaces
{
    public interface ISearchService
    {
        Task<PagedList<AudioViewModel>> SearchAudios(SearchAudiosRequest request,
            CancellationToken cancellationToken = default);

        Task<PagedList<UserViewModel>> SearchUsers(SearchUsersRequest request,
            CancellationToken cancellationToken = default);
    }
}