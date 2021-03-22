using System.Linq;
using Audiochan.Core.Common.Models;
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
                .ForMember(dest => dest.Tags, opts =>
                    opts.MapFrom(src => src.Tags.Select(tag => tag.Id).ToArray()))
                .ForMember(dest => dest.User, opts =>
                    opts.MapFrom(src => new UserDto(src.User.Id, src.User.UserName, src.User.Picture)));
            
            CreateMap<Audio, AudioViewModel>()
                .ForMember(dest => dest.User, opts =>
                    opts.MapFrom(src => new UserDto(src.User.Id, src.User.UserName, src.User.Picture)));
        }
    }
}