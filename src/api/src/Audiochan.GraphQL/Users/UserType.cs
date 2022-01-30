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
            .Type<ListType<AudioType>>()
            .UseDataloader<FavoriteAudiosByUserIdDataLoader>()
            .Resolve(async (ctx, ct) =>
            {
                var parent = ctx.Parent<UserDto>();
                return await ctx.DataLoader<FavoriteAudiosByUserIdDataLoader>().LoadAsync(parent.Id, ct);
            });

        descriptor.Field("followings")
            .Type<ListType<UserType>>()
            .UseDataloader<FollowingByUserIdDataLoader>()
            .Resolve(async (ctx, ct) =>
            {
                var parent = ctx.Parent<UserDto>();
                return await ctx.DataLoader<FollowingByUserIdDataLoader>().LoadAsync(parent.Id, ct);
            });
        
        descriptor.Field("followers")
            .Type<ListType<UserType>>()
            .UseDataloader<FollowerByUserIdDataLoader>()
            .Resolve(async (ctx, ct) =>
            {
                var parent = ctx.Parent<UserDto>();
                return await ctx.DataLoader<FollowerByUserIdDataLoader>().LoadAsync(parent.Id, ct);
            });
    }
    
    private static string? GetUserPicture(IResolverContext context)
    {
        var parent = context.Parent<UserDto>();
        if (string.IsNullOrEmpty(parent.Picture)) return null;
        return MediaLinkConstants.USER_PICTURE + parent.Picture;
    }
}