using Audiochan.Application;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.GraphQL.Audios.DataLoaders;
using Audiochan.GraphQL.Users.DataLoaders;
using HotChocolate.Resolvers;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.GraphQL.Audios;

public class AudioType : ObjectType<Audio>
{
    protected override void Configure(IObjectTypeDescriptor<Audio> descriptor)
    {
        descriptor.ImplementsNode()
            .IdField(x => x.Id)
            .ResolveNode(async (ctx, id) 
                => await ctx.DataLoader<AudioByIdDataLoader>()
                    .LoadAsync(id, ctx.RequestAborted));

        descriptor.Field(x => x.File)
            .Name("audioLink")
            .Resolve(GetAudioLink);

        descriptor.Field(x => x.Picture)
            .Resolve(GetAudioPictureLink);

        descriptor.Field(x => x.UserId)
            .Name("user")
            .Resolve(async (ctx, ct) =>
            {
                var audio = ctx.Parent<Audio>();
                return await ctx.DataLoader<UserByIdDataLoader>().LoadAsync(audio.UserId, ct);
            });

        descriptor.Field("isFavorited")
            .Authorize()
            .UseDbContext<ApplicationDbContext>()
            .Resolve(async (ctx, ct) =>
            {
                var dbContext = ctx.DbContext<ApplicationDbContext>();
                var parent = ctx.Parent<AudioDto>();
                var claimsPrincipal = ctx.GetUser();
                claimsPrincipal.TryGetUserId(out var userId);
                return await dbContext.FavoriteAudios
                    .AnyAsync(x => x.UserId == userId && x.AudioId == parent.Id, ct);
            });

        descriptor.Field(x => x.FavoriteAudios)
            .Name("favorited")
            .UseDbContext<ApplicationDbContext>()
            .Resolve(async (ctx, ct) =>
            {
                var dbContext = ctx.DbContext<ApplicationDbContext>();
                var audio = ctx.Parent<Audio>();
                var favoritedIds = await dbContext.FavoriteAudios
                    .Where(fa => fa.AudioId == audio.Id)
                    .Select(fa => fa.UserId)
                    .ToArrayAsync(ct);
                return await ctx.DataLoader<UserByIdDataLoader>().LoadAsync(favoritedIds, ct);
            });
    }

    private static string GetAudioLink(IResolverContext context)
    {
        var audio = context.Parent<Audio>();
        return MediaLinkConstants.AUDIO_STREAM + audio.File;

    }

    private static string GetAudioPictureLink(IResolverContext context)
    {
        var audio = context.Parent<Audio>();
        return MediaLinkConstants.AUDIO_PICTURE + audio.Picture;
    }
}