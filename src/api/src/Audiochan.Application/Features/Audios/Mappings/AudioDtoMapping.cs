using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Application.Features.Audios.Mappings;

public class AudioDtoMapping : Profile
{
    public AudioDtoMapping()
    {
        this.CreateStrictMap<Audio, AudioDto>()
            .ForMember(dest => dest.Description, c =>
                c.NullSubstitute(""))
            .ForMember(dest => dest.File, c =>
                c.MapFrom(src => src.File));
    }
}