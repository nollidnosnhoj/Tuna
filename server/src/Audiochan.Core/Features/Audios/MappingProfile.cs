using System.Linq;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudio;
using AutoMapper;

namespace Audiochan.Core.Features.Audios
{
    public class AudioMappingProfile : Profile
    {
        public AudioMappingProfile()
        {
            string currentUserId = string.Empty;
            CreateMap<Audio, AudioViewModel>()
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
                        ? new AudioViewModel.GenreDto(src.Genre.Id, src.Genre.Name, src.Genre.Slug)
                        : null))
                .ForMember(dest => dest.User, opts =>
                    opts.MapFrom(src => new UserDto(src.User.Id, src.User.UserName, src.User.Picture)));
        }
    }

    // public static class MappingProfile
    // {
    //     // public static Expression<Func<Entities.Audio, AudioViewModel>> AudioMapToViewmodel(string currentUserId)
    //     // {
    //     //     return audio => new AudioViewModel
    //     //     {
    //     //         Id = audio.Id,
    //     //         Title = audio.Title,
    //     //         Description = audio.Description,
    //     //         IsPublic = audio.IsPublic,
    //     //         IsLoop = audio.IsLoop,
    //     //         Duration = audio.Duration,
    //     //         FileSize = audio.FileSize,
    //     //         FileExt = audio.FileExt,
    //     //         Picture = audio.Picture,
    //     //         Tags = audio.Tags.Select(tag => tag.Id).ToArray(),
    //     //         FavoriteCount = audio.Favorited.Count,
    //     //         IsFavorited = currentUserId != null 
    //     //                       && currentUserId.Length > 0
    //     //                       && audio.Favorited.Any(f => f.UserId == currentUserId),
    //     //         Created = audio.Created,
    //     //         Updated = audio.LastModified,
    //     //         Genre = audio.Genre != null 
    //     //             ? new AudioViewModel.GenreDto(audio.Genre.Id, audio.Genre.Name, audio.Genre.Slug) 
    //     //             : null,
    //     //         User = new UserDto(audio.User.Id, audio.User.UserName, audio.User.Picture),
    //     //         UploadId = audio.UploadId
    //     //     };
    //     // }
    // }
}