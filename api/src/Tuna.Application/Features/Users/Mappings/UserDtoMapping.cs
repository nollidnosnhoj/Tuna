using System.Linq;
using Tuna.Application.Features.Users.Models;
using Tuna.Domain.Entities;

namespace Tuna.Application.Features.Users.Mappings;

public static class Mappings
{
    public static IQueryable<UserDto> ProjectToDto(this IQueryable<User> queryable)
    {
        return queryable.Select(user => new UserDto
        {
            Id = user.Id,
            ImageId = user.ImageId,
            UserName = user.UserName
        });
    }

    public static UserDto MapToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            ImageId = user.ImageId,
            UserName = user.UserName
        };
    }
}