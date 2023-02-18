using System.Linq;
using Audiochan.Common.Helpers;
using Audiochan.Core.Features.Audios.Dtos;
using Audiochan.Core.Features.Users.Dtos;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Features.Audios.Mappings;

public static partial class DtoMappings
{
    public static IQueryable<AudioDto> Project(this IQueryable<Audio> queryable)
    {
        return queryable.Select(x => new AudioDto
        {
            Id = x.Id,
            Description = x.Description ?? "",
            ObjectKey = x.ObjectKey,
            Created = x.Created,
            Duration = x.Duration,
            Picture = x.Picture,
            Size = x.Size,
            Title = x.Title,
            User = new UserDto
            {
                Id = x.UserId,
                Picture = x.User.Picture,
                UserName = x.User.UserName
            }
        });
    }
}