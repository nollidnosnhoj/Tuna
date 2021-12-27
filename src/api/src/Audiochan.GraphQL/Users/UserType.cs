using Audiochan.Application;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Application.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.GraphQL.Audios;
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
            .UsePaging<AudioType>()
            .Resolve(ctx =>
            {
                var dbContext = ctx.DbContext<ApplicationDbContext>();
                var parent = ctx.Parent<UserDto>();
                return dbContext.Audios
                    .Where(a => a.UserId == parent.Id)
                    .OrderByDescending(a => a.Id)
                    .ProjectTo<Audio, AudioDto>(ctx);
            });
        
        descriptor.Field("favoriteAudios")
            .UseDbContext<ApplicationDbContext>()
            .UsePaging<AudioType>()
            .Resolve(ctx =>
            {
                var dbContext = ctx.DbContext<ApplicationDbContext>();
                var parent = ctx.Parent<UserDto>();
                return dbContext.FavoriteAudios
                    .Where(a => a.UserId == parent.Id)
                    .Select(a => a.Audio)
                    .ProjectTo<Audio, AudioDto>(ctx);
            });
        
        descriptor.Field("followings")
            .UseDbContext<ApplicationDbContext>()
            .UsePaging<UserType>()
            .Resolve(ctx =>
            {
                var dbContext = ctx.DbContext<ApplicationDbContext>();
                var parent = ctx.Parent<UserDto>();
                return dbContext.FollowedUsers
                    .Where(a => a.ObserverId == parent.Id)
                    .Select(a => a.Target)
                    .ProjectTo<User, UserDto>(ctx);
            });
        
        descriptor.Field("followers")
            .UseDbContext<ApplicationDbContext>()
            .UsePaging<UserType>()
            .Resolve(ctx =>
            {
                var dbContext = ctx.DbContext<ApplicationDbContext>();
                var parent = ctx.Parent<UserDto>();
                return dbContext.FollowedUsers
                    .Where(a => a.TargetId == parent.Id)
                    .Select(a => a.Observer)
                    .ProjectTo<User, UserDto>(ctx);
            });
    }
    
    private static string? GetUserPicture(IResolverContext context)
    {
        var parent = context.Parent<UserDto>();
        if (string.IsNullOrEmpty(parent.Picture)) return null;
        return MediaLinkConstants.USER_PICTURE + parent.Picture;
    }
}