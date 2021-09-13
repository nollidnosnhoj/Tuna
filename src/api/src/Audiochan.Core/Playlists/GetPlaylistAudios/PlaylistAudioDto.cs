using Audiochan.Core.Audios;
using Audiochan.Core.Common.Mappings;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Core.Playlists.GetPlaylistAudios
{
    public class PlaylistAudioDto : IHasId<long>, IMapFrom<PlaylistAudio>
    {
        public long Id { get; set; }
        public AudioDto Audio { get; set; } = null!;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<PlaylistAudio, PlaylistAudioDto>();
        }
    }
}