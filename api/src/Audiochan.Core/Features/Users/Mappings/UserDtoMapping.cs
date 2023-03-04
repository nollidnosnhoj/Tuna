using System.Linq;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Users.Models;

namespace Audiochan.Core.Features.Users.Mappings;

public static class Mappings
{
    public static IQueryable<UserDto> ProjectToDto(this IQueryable<User> queryable)
    {
        return queryable.Select(user => new UserDto
        {
            Id = user.Id,
            Picture = user.ImageId,
            UserName = user.UserName
        });
    }

    public static UserDto MapToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Picture = user.ImageId,
            UserName = user.UserName
        };
    }
}