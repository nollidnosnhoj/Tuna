using System.Threading;
using System.Threading.Tasks;
using Audiochan.Shared.Mediatr;
using Audiochan.Core.Features.Audios.DataLoaders;
using Audiochan.Core.Features.Audios.Models;
using MediatR;

namespace Audiochan.Core.Features.Audios.Queries;

public record GetAudioQuery(long AudioId) : IQueryRequest<AudioDto?>;

public class GetAudioQueryHandler : IRequestHandler<GetAudioQuery, AudioDto?>
{
    private readonly GetAudioDataLoader _dataLoader;

    public GetAudioQueryHandler(GetAudioDataLoader dataLoader)
    {
        _dataLoader = dataLoader;
    }

    public async Task<AudioDto?> Handle(GetAudioQuery request, CancellationToken cancellationToken)
    {
        return await _dataLoader.LoadAsync(request.AudioId, cancellationToken);
    }
}
