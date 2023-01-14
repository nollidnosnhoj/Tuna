using System.Linq;
using Audiochan.Core.Users.Queries;
using Audiochan.Domain.Entities;

namespace Audiochan.API.Features.Users.Mappings;

public static partial class DtoMappings
{
    public static IQueryable<ProfileDto> ProjectToProfile(this IQueryable<User> queryable)
    {
        return queryable.Select(x => new ProfileDto
        {
            Id = x.Id,
            Picture = x.Picture,
            UserName = x.UserName
        });
    }
}