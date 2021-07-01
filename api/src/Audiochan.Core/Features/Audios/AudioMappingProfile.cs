using System.Linq;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetAudioList;
using AutoMapper;

namespace Audiochan.Core.Features.Audios
{
    public class AudioMappingProfile : Profile
    {
        public AudioMappingProfile()
        {
            CreateMap<Audio, AudioDetailViewModel>()
                .ForMember(dest => dest.Picture, opts =>
                    opts.MapFrom(src => src.PictureBlobName != null
                        ? string.Format(MediaLinkInvariants.AudioPictureUrl, src.PictureBlobName)
                        : null))
                .ForMember(dest => dest.Tags, opts =>
                    opts.MapFrom(src => src.Tags.Select(t => t.Name).ToList()))
                .ForMember(dest => dest.AudioUrl, opts =>
                    opts.MapFrom(src => string.Format(MediaLinkInvariants.AudioUrl, src.Id, src.BlobName)));

            CreateMap<Audio, AudioViewModel>()
                .ForMember(dest => dest.Picture, opts =>
                    opts.MapFrom(src => src.PictureBlobName != null
                        ? string.Format(MediaLinkInvariants.AudioPictureUrl, src.PictureBlobName)
                        : null))
                .ForMember(dest => dest.AudioUrl, opts =>
                    opts.MapFrom(src => string.Format(MediaLinkInvariants.AudioUrl, src.Id, src.BlobName)));
        }
    }
}