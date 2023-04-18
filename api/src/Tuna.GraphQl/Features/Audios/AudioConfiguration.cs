using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tuna.Application.Features.Audios.DataLoaders;

namespace Tuna.GraphQl.Features.Audios;

public static class AudioConfiguration
{
    public static IRequestExecutorBuilder AddAudioFeatures(this IRequestExecutorBuilder builder)
    {
        return builder
            .AddTypeExtension<AudioNode>()
            .AddTypeExtension<AudioExtensions>()
            .AddTypeExtension<AudioQueries>()
            .AddTypeExtension<AudioMutations>()
            .AddDataLoader<GetAudioDataLoader>();
    }
}