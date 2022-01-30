using Audiochan.Application;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Persistence;
using Audiochan.GraphQL.Audios.DataLoaders;
using Audiochan.GraphQL.Users.DataLoaders;
using HotChocolate.Resolvers;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.GraphQL.Audios;

public class AudioType : ObjectType<AudioDto>
{
    protected override void Configure(IObjectTypeDescriptor<AudioDto> descriptor)
    {
        descriptor.Name("Audio");
        
        descriptor.ImplementsNode()
            .IdField(x => x.Id)
            .ResolveNode(async (ctx, id) 
                => await ctx.DataLoader<AudioByIdDataLoader>()
                    .LoadAsync(id, ctx.RequestAborted));

        descriptor.Field(x => x.File)
            .Name("mp3")
            .Resolve(GetAudioLink);

        descriptor.Field(x => x.Picture)
            .Resolve(GetAudioPictureLink);

        descriptor.Ignore(x => x.UserId);
        descriptor.Field(x => x.User)
            .Resolve(async (ctx, ct) =>
            {
                var audio = ctx.Parent<AudioDto>();
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

        descriptor.Field("favorited")
            .UseDbContext<ApplicationDbContext>()
            .Resolve(async (ctx, ct) =>
            {
                var dbContext = ctx.DbContext<ApplicationDbContext>();
                var audio = ctx.Parent<AudioDto>();
                var favoritedIds = await dbContext.FavoriteAudios
                    .Where(fa => fa.AudioId == audio.Id)
                    .Select(fa => fa.UserId)
                    .ToArrayAsync(ct);
                return await ctx.DataLoader<UserByIdDataLoader>().LoadAsync(favoritedIds, ct);
            });
    }

    private static string GetAudioLink(IResolverContext context)
    {
        var audio = context.Parent<AudioDto>();
        return MediaLinkConstants.AUDIO_STREAM + audio.File;

    }

    private static string? GetAudioPictureLink(IResolverContext context)
    {
        var audio = context.Parent<AudioDto>();
        if (string.IsNullOrEmpty(audio.Picture)) return null;
        return MediaLinkConstants.AUDIO_PICTURE + audio.Picture;
    }
}