using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Features.Users.GetUser;

namespace Audiochan.Core.Features.Search.SearchAll
{
    public record SearchAllViewModel
    {
        public PagedList<AudioViewModel> Audios { get; init; }
        public PagedList<UserViewModel> Users { get; init; }
    }
}