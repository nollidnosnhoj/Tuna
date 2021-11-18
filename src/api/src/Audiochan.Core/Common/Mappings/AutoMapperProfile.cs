using System.Linq;
using Audiochan.Core.Audios;
using Audiochan.Core.Auth;
using Audiochan.Core.Auth.Queries;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Users;
using Audiochan.Core.Users.Queries;
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
                .ForMember(dest => dest.Src, c =>
                    c.MapFrom(src => src.File))
                .ForMember(dest => dest.IsFavorited, c =>
                    c.MapFrom(src => userId > 0 ? src.FavoriteAudios.Any(fa => fa.UserId == userId) : (bool?)null));
            CreateStrictMap<Artist, ArtistProfileDto>();
            CreateStrictMap<Artist, ArtistDto>();
            CreateStrictMap<User, CurrentUserDto>()
                .ForMember(dest => dest.IsArtist, opts =>
                    opts.MapFrom(src => src.UserType == UserTypes.ARTIST));
        }

        private IMappingExpression<TSource, TDestination> CreateStrictMap<TSource, TDestination>()
            where TDestination : IMapFrom<TSource>
        {
            return CreateMap<TSource, TDestination>();
        }
    }
}