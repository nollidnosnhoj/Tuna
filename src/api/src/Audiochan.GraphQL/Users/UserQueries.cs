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

    public async Task<UserDto> GetUserByName(
        string userName,
        UserByNameDataLoader userByName,
        CancellationToken cancellationToken = default)
    {
        return await userByName.LoadAsync(userName, cancellationToken);
    }

    [UseApplicationDbContext]
    [UsePaging]
    public IQueryable<UserDto> GetFollowings(
        string userName,
        IResolverContext context,
        [ScopedService] ApplicationDbContext dbContext)
    {
        return dbContext.Users
            .Where(u => u.UserName == userName)
            .SelectMany(u => u.Followings)
            .OrderByDescending(fu => fu.FollowedDate)
            .Select(fu => fu.Target)
            .ProjectTo<User, UserDto>(context);
    }
    
    [UseApplicationDbContext]
    [UsePaging]
    public IQueryable<UserDto> GetFollowers(
        string userName,
        IResolverContext context,
        [ScopedService] ApplicationDbContext dbContext)
    {
        return dbContext.Users
            .Where(u => u.UserName == userName)
            .SelectMany(u => u.Followers)
            .OrderByDescending(fu => fu.FollowedDate)
            .Select(fu => fu.Observer)
            .ProjectTo<User, UserDto>(context);
    }
    
    [Authorize]
    [UseApplicationDbContext]
    [UsePaging]
    public IQueryable<UserDto> GetYourFollowings(
        ClaimsPrincipal claimsPrincipal,
        IResolverContext context,
        [ScopedService] ApplicationDbContext dbContext)
    {
        var userId = claimsPrincipal.GetUserId();
        return dbContext.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Followings)
            .OrderByDescending(fu => fu.FollowedDate)
            .Select(fu => fu.Target)
            .ProjectTo<User, UserDto>(context);
    }
    
    [Authorize]
    [UseApplicationDbContext]
    [UsePaging]
    public IQueryable<UserDto> GetYourFollowers(
        ClaimsPrincipal claimsPrincipal,
        IResolverContext context,
        [ScopedService] ApplicationDbContext dbContext)
    {
        var userId = claimsPrincipal.GetUserId();
        return dbContext.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Followers)
            .OrderByDescending(fu => fu.FollowedDate)
            .Select(fu => fu.Observer)
            .ProjectTo<User, UserDto>(context);
    }
}