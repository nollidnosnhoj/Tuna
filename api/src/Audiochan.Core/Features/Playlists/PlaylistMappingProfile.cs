using Audiochan.Core.Common.Constants;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Playlists.GetPlaylistDetail;
using AutoMapper;

namespace Audiochan.Core.Features.Playlists
{
    public class PlaylistMappingProfile : Profile
    {
        public PlaylistMappingProfile()
        {
            CreateMap<Playlist, PlaylistDetailViewModel>()
                .ForMember(dest => dest.Description, opts =>
                    opts.NullSubstitute(""))
                .ForMember(dest => dest.Picture, opts =>
                    opts.MapFrom(src => src.Picture != null
                        ? string.Format(MediaLinkInvariants.PlaylistPictureUrl, src.Picture)
                        : null));
        }
    }
}