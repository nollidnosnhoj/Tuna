using System.Linq;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Features.Users.Mappings;

public static class Mappings
{
    public static IQueryable<UserViewModel> ProjectToDto(this IQueryable<User> queryable)
    {
        return queryable.Select(user => new UserViewModel
        {
            Id = user.Id,
            Picture = user.ImageId,
            UserName = user.UserName
        });
    }

    public static UserViewModel MapToDto(this User user)
    {
        return new UserViewModel
        {
            Id = user.Id,
            Picture = user.ImageId,
            UserName = user.UserName
        };
    }
}