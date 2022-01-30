using Audiochan.Application;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Application.Persistence;
using Audiochan.GraphQL.Audios.DataLoaders;
using Audiochan.GraphQL.Users.DataLoaders;
using HotChocolate.Resolvers;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.GraphQL.Users;

public class UserType : ObjectType<UserDto>
{
    protected override void Configure(IObjectTypeDescriptor<UserDto> descriptor)
    {
        descriptor.Name("User");
        
        descriptor.ImplementsNode()
            .IdField(x => x.Id)
            .ResolveNode(async (ctx, id) 
                => await ctx.DataLoader<UserByIdDataLoader>().LoadAsync(id, ctx.RequestAborted));

        descriptor.Field(x => x.Picture)
            .Resolve(GetUserPicture);

        descriptor.Field("isFollowed")
            .Authorize()
            .UseDbContext<ApplicationDbContext>()
            .Resolve(async (ctx, ct) =>
            {
                var dbContext = ctx.DbContext<ApplicationDbContext>();
                var parent = ctx.Parent<UserDto>();
                var userId = ctx.GetUser().GetUserId();
                return await dbContext.FollowedUsers
                    .AnyAsync(fu => fu.ObserverId == userId && fu.TargetId == parent.Id, ct);
            });

        descriptor.Field("audios")
            .UseDbContext<ApplicationDbContext>()
            .Resolve(async (ctx, ct) =>
            {
                var dbContext = ctx.DbContext<ApplicationDbContext>();
                var user = ctx.Parent<UserDto>();
                var audioIds = await dbContext.Audios
                    .Where(a => a.UserId == user.Id)
                    .Select(a => a.Id)
                    .ToArrayAsync(ct);
                return await ctx.DataLoader<AudioByIdDataLoader>().LoadAsync(audioIds, ct);
            });
        
        descriptor.Field("favoriteAudios")
            .UseDbContext<ApplicationDbContext>()
            .Resolve(async (ctx, ct) =>
            {
                var dbContext = ctx.DbContext<ApplicationDbContext>();
                var user = ctx.Parent<UserDto>();
                var audioIds = await dbContext.FavoriteAudios
                    .Where(a => a.UserId == user.Id)
                    .Select(a => a.AudioId)
                    .ToArrayAsync(ct);
                return await ctx.DataLoader<AudioByIdDataLoader>().LoadAsync(audioIds, ct);
            });
        
        descriptor.Field("followings")
            .UseDbContext<ApplicationDbContext>()
            .Resolve(async (ctx, ct) =>
            {
                var dbContext = ctx.DbContext<ApplicationDbContext>();
                var user = ctx.Parent<UserDto>();
                var followingIds = await dbContext.FollowedUsers
                    .Where(a => a.ObserverId == user.Id)
                    .Select(a => a.TargetId)
                    .ToArrayAsync(ct);
                return await ctx.DataLoader<UserByIdDataLoader>().LoadAsync(followingIds, ct);
            });
        
        descriptor.Field("followers")
            .UseDbContext<ApplicationDbContext>()
            .Resolve(async (ctx, ct) =>
            {
                var dbContext = ctx.DbContext<ApplicationDbContext>();
                var user = ctx.Parent<UserDto>();
                var followerIds = await dbContext.FollowedUsers
                    .Where(a => a.TargetId == user.Id)
                    .Select(a => a.ObserverId)
                    .ToArrayAsync(ct);
                return await ctx.DataLoader<UserByIdDataLoader>().LoadAsync(followerIds, ct);
            });
    }
    
    private static string? GetUserPicture(IResolverContext context)
    {
        var user = context.Parent<UserDto>();
        if (string.IsNullOrEmpty(user.Picture)) return null;
        return MediaLinkConstants.USER_PICTURE + user.Picture;
    }
}