using System.Linq;
using Audiochan.Core.Features.Users.Dtos;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Features.Users.Mappings;

public static partial class DtoMappings
{
    public static IQueryable<UserDto> ProjectToUser(this IQueryable<User> queryable)
    {
        return queryable.Select(x => new UserDto
        {
            Id = x.Id,
            Picture = x.ImageId,
            UserName = x.UserName
        });
    }
}