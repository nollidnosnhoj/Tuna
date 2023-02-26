using System.Linq;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Users.Models;

namespace Audiochan.Core.Features.Users.Mappings;

public static class Mappings
{
    public static IQueryable<UserViewModel> ProjectToUser(this IQueryable<User> queryable)
    {
        return queryable.Select(x => new UserViewModel
        {
            Id = x.Id,
            Picture = x.ImageId,
            UserName = x.UserName
        });
    }
}