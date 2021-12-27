using System.Security.Claims;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Application.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.GraphQL.Common.Attributes;
using Audiochan.GraphQL.Users.DataLoaders;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Resolvers;

namespace Audiochan.GraphQL.Users;

[ExtendObjectType(OperationTypeNames.Query)]
public class UserQueries
{
    [Authorize]
    public async Task<UserDto> GetMe(ClaimsPrincipal claimsPrincipal, UserByIdDataLoader userById,
        CancellationToken cancellationToken = default)
    {
        claimsPrincipal.TryGetUserId(out var userId);
        return await userById.LoadAsync(userId, cancellationToken);
    }

    [UseApplicationDbContext]
    [UseSingleOrDefault]
    public IQueryable<UserDto> GetUserByName(
        string userName,
        IResolverContext context,
        [ScopedService] ApplicationDbContext dbContext)
    {
        return dbContext.Users
            .Where(u => u.UserName == userName)
            .ProjectTo<User, UserDto>(context);
    }
}