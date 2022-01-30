using Audiochan.Application;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.GraphQL.Audios.DataLoaders;
using Audiochan.GraphQL.Users.DataLoaders;
using HotChocolate.Resolvers;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.GraphQL.Users;

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.ImplementsNode()
            .IdField(x => x.Id)
            .ResolveNode(async (ctx, id) 
                => await ctx.DataLoader<UserByIdDataLoader>().LoadAsync(id, ctx.RequestAborted));

        descriptor.Ignore(x => x.Email);
        descriptor.Ignore(x => x.PasswordHash);
        descriptor.Ignore(x => x.Role);
        descriptor.Ignore(x => x.LastModified);

        descriptor.Field(x => x.Picture)
            .Resolve(GetUserPicture);

        descriptor.Field("isFollowed")
            .Authorize()
            .UseDbContext<ApplicationDbContext>()
            .Resolve(async (ctx, ct) =>
            {
                var dbContext = ctx.DbContext<ApplicationDbContext>();
                var parent = ctx.Parent<User>();
                var userId = ctx.GetUser().GetUserId();
                return await dbContext.FollowedUsers
                    .AnyAsync(fu => fu.ObserverId == userId && fu.TargetId == parent.Id, ct);
            });

        descriptor.Field(x => x.Audios)
            .UseDbContext<ApplicationDbContext>()
            .Resolve(async (ctx, ct) =>
            {
                var dbContext = ctx.DbContext<ApplicationDbContext>();
                var user = ctx.Parent<User>();
                var audioIds = await dbContext.Audios
                    .Where(a => a.UserId == user.Id)
                    .Select(a => a.Id)
                    .ToArrayAsync(ct);
                return await ctx.DataLoader<AudioByIdDataLoader>().LoadAsync(audioIds, ct);
            });
        
        descriptor.Field(x => x.FavoriteAudios)
            .UseDbContext<ApplicationDbContext>()
            .Resolve(async (ctx, ct) =>
            {
                var dbContext = ctx.DbContext<ApplicationDbContext>();
                var user = ctx.Parent<User>();
                var audioIds = await dbContext.FavoriteAudios
                    .Where(a => a.UserId == user.Id)
                    .Select(a => a.AudioId)
                    .ToArrayAsync(ct);
                return await ctx.DataLoader<AudioByIdDataLoader>().LoadAsync(audioIds, ct);
            });
        
        descriptor.Field(x => x.Followings)
            .UseDbContext<ApplicationDbContext>()
            .Resolve(async (ctx, ct) =>
            {
                var dbContext = ctx.DbContext<ApplicationDbContext>();
                var user = ctx.Parent<User>();
                var followingIds = await dbContext.FollowedUsers
                    .Where(a => a.ObserverId == user.Id)
                    .Select(a => a.TargetId)
                    .ToArrayAsync(ct);
                return await ctx.DataLoader<UserByIdDataLoader>().LoadAsync(followingIds, ct);
            });
        
        descriptor.Field(x => x.Followers)
            .UseDbContext<ApplicationDbContext>()
            .Resolve(async (ctx, ct) =>
            {
                var dbContext = ctx.DbContext<ApplicationDbContext>();
                var user = ctx.Parent<User>();
                var followerIds = await dbContext.FollowedUsers
                    .Where(a => a.TargetId == user.Id)
                    .Select(a => a.ObserverId)
                    .ToArrayAsync(ct);
                return await ctx.DataLoader<UserByIdDataLoader>().LoadAsync(followerIds, ct);
            });
    }
    
    private static string GetUserPicture(IResolverContext context)
    {
        var user = context.Parent<User>();
        return MediaLinkConstants.USER_PICTURE + user.Picture;
    }
}