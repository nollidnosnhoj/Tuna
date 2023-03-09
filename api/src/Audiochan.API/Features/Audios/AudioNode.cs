using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Audios.DataLoaders;
using Audiochan.Core.Features.Audios.Models;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace Audiochan.API.Features.Audios;

[Node]
[ExtendObjectType(typeof(AudioDto))]
public sealed class AudioNode
{
    [NodeResolver]
    public static Task<AudioDto> GetAudioAsync(long id, GetAudioDataLoader dataLoader,
        CancellationToken cancellationToken)
    {
        return dataLoader.LoadAsync(id, cancellationToken);
    }
}