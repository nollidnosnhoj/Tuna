using System.Security.Claims;
using Audiochan.Application;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Users.Models;
using Audiochan.GraphQL.Audios.DataLoaders;
using Audiochan.GraphQL.Users.DataLoaders;

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
            .Resolve(context =>
            {
                var parent = context.Parent<UserDto>();
                if (string.IsNullOrEmpty(parent.Picture)) return null;
                return MediaLinkConstants.USER_PICTURE + parent.Picture;
            });

        descriptor.Field(x => x.Email)
            .Authorize()
            .Resolve(ctx =>
            {
                var parent = ctx.Parent<UserDto>();
                var myEmail = ctx.GetUser().FindFirstValue(ClaimTypes.Email);
                return myEmail == parent.Email ? parent.Email : null;
            });

        descriptor.Field("isFollowed")
            .Type<BooleanType>()
            .Authorize()
            // .UseDataloader<FollowerByUserIdDataLoader>()
            .Resolve(async (ctx, ct) =>
            {
                var userId = ctx.GetUser().GetUserId();
                var parent = ctx.Parent<UserDto>();
                var followers = await ctx.DataLoader<FollowerByUserIdDataLoader>().LoadAsync(parent.Id, ct);
                return followers.Any(u => u.Id == userId);
            });

        descriptor.Field("audios")
            .UseDataloader<AudiosByUserIdDataLoader>()
            .Resolve(async (ctx, ct) =>
            {
                var parent = ctx.Parent<UserDto>();
                return await ctx.DataLoader<AudiosByUserIdDataLoader>().LoadAsync(parent.Id, ct);
            });
        
        descriptor.Field("favoriteAudios")
            .UseDataloader<FavoriteAudiosByUserIdDataLoader>()
            .Resolve(async (ctx, ct) =>
            {
                var parent = ctx.Parent<UserDto>();
                return await ctx.DataLoader<FavoriteAudiosByUserIdDataLoader>().LoadAsync(parent.Id, ct);
            });

        descriptor.Field("followings")
            .UseDataloader<FollowingByUserIdDataLoader>()
            .Resolve(async (ctx, ct) =>
            {
                var parent = ctx.Parent<UserDto>();
                return await ctx.DataLoader<FollowingByUserIdDataLoader>().LoadAsync(parent.Id, ct);
            });
        
        descriptor.Field("followers")
            .UseDataloader<FollowerByUserIdDataLoader>()
            .Resolve(async (ctx, ct) =>
            {
                var parent = ctx.Parent<UserDto>();
                return await ctx.DataLoader<FollowerByUserIdDataLoader>().LoadAsync(parent.Id, ct);
            });
    }
}