using System.Linq;
using Audiochan.Common.Helpers;
using Audiochan.Core.Features.Audios.Dtos;
using Audiochan.Core.Features.Users.Dtos;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Features.Audios.Mappings;

public static partial class DtoMappings
{
    public static IQueryable<AudioDto> Project(this IQueryable<Audio> queryable, long? userId)
    {
        return queryable.Select(x => new AudioDto
        {
            Id = x.Id,
            Description = x.Description ?? "",
            Src = x.File,
            IsFavorited = userId > 0
                ? x.FavoriteAudios.Any(fa => fa.UserId == userId)
                : null,
            Slug = HashIdHelper.EncodeLong(x.Id),
            Created = x.Created,
            Duration = x.Duration,
            Picture = x.Picture,
            Size = x.Size,
            Tags = x.Tags,
            Title = x.Title,
            User = new UserDto
            {
                Id = x.UserId,
                Picture = x.User.Picture,
                UserName = x.User.UserName
            }
        });
    }

    public static AudioDto Map(this AudioDto audio)
    {
        audio.Src = MediaLinkConstants.AUDIO_STREAM + audio.Src;
        audio.Picture = MediaLinkConstants.AUDIO_PICTURE + audio.Picture;
        return audio;
    }
}