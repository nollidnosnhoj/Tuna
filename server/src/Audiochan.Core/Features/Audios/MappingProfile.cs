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
            var currentUserId = string.Empty;
            CreateMap<Audio, AudioDetailViewModel>()
                .ForMember(dest => dest.Tags, opts =>
                    opts.MapFrom(src => src.Tags.Select(tag => tag.Id).ToArray()))
                .ForMember(dest => dest.FavoriteCount, opts =>
                    opts.MapFrom(src => src.Favorited.Count))
                .ForMember(dest => dest.IsFavorited, opts =>
                    opts.MapFrom(src =>
                        currentUserId != null && currentUserId.Length > 0 &&
                        src.Favorited.Any(f => f.UserId == currentUserId)))
                .ForMember(dest => dest.Genre, opts =>
                    opts.MapFrom(src => src.Genre != null
                        ? new GenreDto(src.Genre.Id, src.Genre.Name, src.Genre.Slug)
                        : null))
                .ForMember(dest => dest.User, opts =>
                    opts.MapFrom(src => new UserDto(src.User.Id, src.User.UserName, src.User.Picture)));
            
            CreateMap<Audio, AudioViewModel>()
                .ForMember(dest => dest.FavoriteCount, opts =>
                    opts.MapFrom(src => src.Favorited.Count))
                .ForMember(dest => dest.IsFavorited, opts =>
                    opts.MapFrom(src =>
                        currentUserId != null && currentUserId.Length > 0 &&
                        src.Favorited.Any(f => f.UserId == currentUserId)))
                .ForMember(dest => dest.Genre, opts =>
                    opts.MapFrom(src => src.Genre != null
                        ? new GenreDto(src.Genre.Id, src.Genre.Name, src.Genre.Slug)
                        : null))
                .ForMember(dest => dest.User, opts =>
                    opts.MapFrom(src => new UserDto(src.User.Id, src.User.UserName, src.User.Picture)));
        }
    }
}