using System;
using System.Linq;
using System.Reflection;
using Audiochan.Core.Audios;
using Audiochan.Core.Auth.GetCurrentUser;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Playlists;
using Audiochan.Core.Users;
using Audiochan.Core.Users.GetProfile;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Core.Common.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            long? userId = null;
            CreateStrictMap<Audio, AudioDto>()
                .ForMember(dest => dest.Description, c =>
                    c.NullSubstitute(""))
                .ForMember(dest => dest.Slug, c =>
                    c.MapFrom(src => HashIdHelper.EncodeLong(src.Id)))
                .ForMember(dest => dest.Audio, c =>
                    c.MapFrom(src => src.File))
                .ForMember(dest => dest.IsFavorited, c =>
                    c.MapFrom(src => userId > 0 ? src.FavoriteAudios.Any(fa => fa.UserId == userId) : (bool?)null));
            CreateStrictMap<Playlist, PlaylistDto>()
                .ForMember(dest => dest.Description, c =>
                    c.NullSubstitute(""));
            CreateStrictMap<User, ProfileDto>();
            CreateStrictMap<User, UserDto>();
            CreateStrictMap<User, CurrentUserDto>();
        }

        private IMappingExpression<TSource, TDestination> CreateStrictMap<TSource, TDestination>()
            where TDestination : IMapFrom<TSource>
        {
            return CreateMap<TSource, TDestination>();
        }
    }
}