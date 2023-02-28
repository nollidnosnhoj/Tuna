using Audiochan.Core;
using Audiochan.Core.Features.Audios.DataLoaders;
using Audiochan.Core.Features.Audios.Models;
using HashidsNet;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.API.Features.Audios;

public class AudioType : ObjectType<AudioDto>
{
    protected override void Configure(IObjectTypeDescriptor<AudioDto> descriptor)
    {
        descriptor.Name("Audio");

        descriptor.ImplementsNode()
            .IdField(x => x.Id)
            .ResolveNode(async (context, id) =>
            {
                var dataLoader = context.DataLoader<GetAudioDataLoader>();
                return await dataLoader.LoadAsync(id, context.RequestAborted);
            });

        descriptor.Field("slug")
            .Resolve(context =>
            {
                var hashids = context.Services.GetRequiredService<IHashids>();
                var audio = context.Parent<AudioDto>();
                return hashids.EncodeLong(audio.Id);
            });

        descriptor.Field(x => x.ObjectKey)
            .Name("streamUrl")
            .Resolve(context =>
            {
                var audio = context.Parent<AudioDto>();
                return MediaLinkConstants.AUDIO_STREAM + audio.ObjectKey;
            });
        
        descriptor.Field(x => x.Picture)
            .Name("imageUrl")
            .Resolve(context =>
            {
                var audio = context.Parent<AudioDto>();
                if (audio.Picture is null) return null;
                return MediaLinkConstants.AUDIO_PICTURE + audio.Picture;
            });
    }
}