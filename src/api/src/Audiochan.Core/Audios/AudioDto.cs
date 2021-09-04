using System;
using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Users;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Core.Audios
{
    public record AudioDto : IHasId<long>, IMapFrom<Audio>
    {
        public long Id { get; init; }
        public string Title { get; init; } = null!;
        public string Slug { get; init; } = null!;
        public string Description { get; init; } = string.Empty;
        public List<string> Tags { get; init; } = new();
        public decimal Duration { get; init; }
        public long Size { get; init; }
        public string? Picture { get; init; }
        public bool? IsFavorited { get; init; }
        public DateTime Created { get; init; }
        public DateTime? LastModified { get; init; }
        public string Audio { get; init; } = null!;
        public UserDto User { get; init; } = null!;

        public void Mapping(Profile profile)
        {
            long? userId = null;
            profile.CreateMap<Audio, AudioDto>()
                .ForMember(dest => dest.Description, c =>
                    c.NullSubstitute(""))
                .ForMember(dest => dest.Slug, c =>
                    c.MapFrom(src => HashIdHelper.EncodeLong(src.Id)))
                .ForMember(dest => dest.Picture, c =>
                {
                    c.MapFrom(src => src.Picture != null ? MediaLinkConstants.AudioPicture + src.Picture : null);
                })
                .ForMember(dest => dest.Audio, c =>
                    c.MapFrom(src => MediaLinkConstants.AudioStream + src.File))
                .ForMember(dest => dest.Tags, c =>
                    c.MapFrom(src => src.Tags.Select(t => t.Name).ToList()))
                .ForMember(dest => dest.IsFavorited, c =>
                    c.MapFrom(src => userId > 0 ? src.FavoriteAudios.Any(fa => fa.UserId == userId) : (bool?)null));
        }
    }
}