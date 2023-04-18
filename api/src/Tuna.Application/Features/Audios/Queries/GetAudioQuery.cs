using System.Threading;
using System.Threading.Tasks;
using Tuna.Shared.Mediatr;
using MediatR;
using Tuna.Application.Features.Audios.DataLoaders;
using Tuna.Application.Features.Audios.Models;

namespace Tuna.Application.Features.Audios.Queries;

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
