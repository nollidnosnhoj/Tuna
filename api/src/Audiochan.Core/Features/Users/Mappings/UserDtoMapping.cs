using System.Linq;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Features.Users.Mappings;

public static partial class DtoMappings
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