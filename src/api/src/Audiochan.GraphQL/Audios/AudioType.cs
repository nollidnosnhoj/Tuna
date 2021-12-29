using Audiochan.Application;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Application.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.GraphQL.Audios.DataLoaders;
using Audiochan.GraphQL.Users;
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
                var parent = ctx.Parent<AudioDto>();
                return await ctx.DataLoader<UserByIdDataLoader>().LoadAsync(parent.UserId, ct);
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
            .UseDataloader<FavoritedByAudioIdDataLoader>()
            .Resolve(async (ctx, ct) =>
            {
                var parent = ctx.Parent<AudioDto>();
                return await ctx.DataLoader<FavoritedByAudioIdDataLoader>().LoadAsync(parent.Id, ct);
            });
    }

    private static string GetAudioLink(IResolverContext context)
    {
        var parent = context.Parent<AudioDto>();
        return MediaLinkConstants.AUDIO_STREAM + parent.File;

    }

    private static string? GetAudioPictureLink(IResolverContext context)
    {
        var parent = context.Parent<AudioDto>();
        if (string.IsNullOrEmpty(parent.Picture)) return null;
        return MediaLinkConstants.AUDIO_PICTURE + parent.Picture;
    }
}