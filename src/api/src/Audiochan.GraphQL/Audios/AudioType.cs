using Audiochan.Application;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.GraphQL.Audios.DataLoaders;
using Audiochan.GraphQL.Users.DataLoaders;

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
            .Resolve(context =>
            {
                var parent = context.Parent<AudioDto>();
                return MediaLinkConstants.AUDIO_STREAM + parent.File;
            });

        descriptor.Field(x => x.Picture)
            .Resolve(context =>
            {
                var parent = context.Parent<AudioDto>();
                if (string.IsNullOrEmpty(parent.Picture)) return null;
                return MediaLinkConstants.AUDIO_PICTURE + parent.Picture;
            });

        descriptor.Ignore(x => x.UserId);
        descriptor.Field(x => x.User)
            .Resolve(async (ctx, ct) =>
            {
                var parent = ctx.Parent<AudioDto>();
                return await ctx.DataLoader<UserByIdDataLoader>().LoadAsync(parent.UserId, ct);
            });

        descriptor.Field("isFavorited")
            .Type<BooleanType>()
            .Authorize()
            // .UseDataloader<FavoritedByAudioIdDataLoader>()
            .Resolve(async (ctx, ct) =>
            {
                var currentUserId = ctx.GetUser().GetUserId();
                var parent = ctx.Parent<AudioDto>();
                var favorited = await ctx.DataLoader<FavoritedByAudioIdDataLoader>().LoadAsync(parent.Id, ct);
                return favorited.Any(u => u.Id == currentUserId);
            });

        descriptor.Field("favorited")
            .UseDataloader<FavoritedByAudioIdDataLoader>()
            .Resolve(async (ctx, ct) =>
            {
                var parent = ctx.Parent<AudioDto>();
                return await ctx.DataLoader<FavoritedByAudioIdDataLoader>().LoadAsync(parent.Id, ct);
            });
    }
}