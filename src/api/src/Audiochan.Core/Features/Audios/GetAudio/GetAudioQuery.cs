using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Audios.GetAudio
{
    public record GetAudioQuery(long Id) : IRequest<AudioDto?>
    {
    }

    public sealed class GetAudioSpecification : Specification<Audio, AudioDto>
    {
        public GetAudioSpecification(long id)
        {
            Query.AsNoTracking();
            Query.Where(x => x.Id == id);
            Query.Select(AudioMaps.AudioToView());
        }
    }

    public class GetAudioQueryHandler : IRequestHandler<GetAudioQuery, AudioDto?>
    {
        private readonly ICacheService _cacheService;
        private readonly IUnitOfWork _unitOfWork;

        public GetAudioQueryHandler(ICacheService cacheService, IUnitOfWork unitOfWork)
        {
            _cacheService = cacheService;
            _unitOfWork = unitOfWork;
        }

        public async Task<AudioDto?> Handle(GetAudioQuery query, CancellationToken cancellationToken)
        {
            var cacheOptions = new GetAudioCacheOptions(query.Id);
            
            var (cacheExists, audio) = await _cacheService
                .GetAsync<AudioDto>(cacheOptions, cancellationToken);

            if (cacheExists) return audio;

            audio = await _unitOfWork.Audios.GetFirstAsync(new GetAudioSpecification(query.Id), cancellationToken);
            
            await _cacheService.SetAsync(audio, cacheOptions, cancellationToken);

            return audio;
        }
    }
}