using System.Security.Claims;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Domain.Entities;
using Audiochan.GraphQL.Users.DataLoaders;
using HotChocolate.AspNetCore.Authorization;

namespace Audiochan.GraphQL.Users;

[ExtendObjectType(OperationTypeNames.Query)]
public class UserQueries
{
    [Authorize]
    public async Task<User> GetMe(ClaimsPrincipal claimsPrincipal, UserByIdDataLoader userById,
        CancellationToken cancellationToken = default)
    {
        claimsPrincipal.TryGetUserId(out var userId);
        return await userById.LoadAsync(userId, cancellationToken);
    }
}