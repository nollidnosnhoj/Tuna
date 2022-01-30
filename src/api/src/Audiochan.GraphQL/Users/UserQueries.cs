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
    
    public async Task<UserDto> GetUserById(
        [ID(nameof(UserDto))] long id,
        UserByIdDataLoader userById,
        CancellationToken cancellationToken = default)
    {
        return await userById.LoadAsync(id, cancellationToken);
    }
    
    public async Task<IEnumerable<UserDto>> GetUsersByIds(
        [ID(nameof(UserDto))] long[] ids,
        UserByIdDataLoader userById,
        CancellationToken cancellationToken = default)
    {
        return await userById.LoadAsync(ids, cancellationToken);
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

    [UseApplicationDbContext]
    [UseOffsetPaging]
    public IQueryable<UserDto> GetFollowings(
        [ID(nameof(UserDto))] long id,
        IResolverContext context,
        [ScopedService] ApplicationDbContext dbContext)
    {
        return dbContext.FollowedUsers
            .Where(fu => fu.ObserverId == id)
            .OrderByDescending(fu => fu.FollowedDate)
            .Select(fu => fu.Target)
            .ProjectTo<User, UserDto>(context);
    }
    
    [UseApplicationDbContext]
    [UseOffsetPaging]
    public IQueryable<UserDto> GetFollowers(
        [ID(nameof(UserDto))] long id,
        IResolverContext context,
        [ScopedService] ApplicationDbContext dbContext)
    {
        return dbContext.FollowedUsers
            .Where(fu => fu.TargetId == id)
            .OrderByDescending(fu => fu.FollowedDate)
            .Select(fu => fu.Observer)
            .ProjectTo<User, UserDto>(context);
    }
}