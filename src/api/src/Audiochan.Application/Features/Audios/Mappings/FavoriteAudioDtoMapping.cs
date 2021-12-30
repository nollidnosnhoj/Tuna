using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Application.Features.Audios.Mappings;

public class FavoriteAudioDtoMapping : Profile
{
    public FavoriteAudioDtoMapping()
    {
        this.CreateStrictMap<FavoriteAudio, FavoriteAudioDto>();
    }
}