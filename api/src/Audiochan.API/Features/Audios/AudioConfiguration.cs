using Audiochan.Core.Features.Audios.DataLoaders;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.API.Features.Audios;

public static class AudioConfiguration
{
    public static IRequestExecutorBuilder AddAudioFeatures(this IRequestExecutorBuilder builder)
    {
        return builder
            .AddTypeExtension<AudioNode>()
            .AddTypeExtension<AudioQueries>()
            .AddTypeExtension<AudioMutations>()
            .AddDataLoader<GetAudioDataLoader>();
    }
}