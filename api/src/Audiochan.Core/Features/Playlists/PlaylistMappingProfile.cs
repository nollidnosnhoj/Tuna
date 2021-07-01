using Audiochan.Core.Entities;
using Audiochan.Core.Features.Playlists.GetPlaylist;
using AutoMapper;

namespace Audiochan.Core.Features.Playlists
{
    public class PlaylistMappingProfile : Profile
    {
        public PlaylistMappingProfile()
        {
            CreateMap<Playlist, PlaylistDetailViewModel>();
        }
    }
}