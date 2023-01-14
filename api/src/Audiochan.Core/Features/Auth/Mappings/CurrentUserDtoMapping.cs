using System.Linq;
using Audiochan.Core.Features.Auth.Queries.GetCurrentUser;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Features.Auth.Mappings;

public static partial class DtoMappings
{
    public static IQueryable<CurrentUserDto> Project(this IQueryable<User> queryable)
    {
        return queryable.Select(x => new CurrentUserDto
        {
            Id = x.Id,
            Email = x.Email,
            UserName = x.UserName
        });
    }
}