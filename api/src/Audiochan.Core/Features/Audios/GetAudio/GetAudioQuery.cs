using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Audios.GetAudio
{
    public record GetAudioQuery(long Id, string? PrivateKey = null) : IRequest<AudioDetailViewModel?>
    {
    }

    public class GetAudioQueryHandler : IRequestHandler<GetAudioQuery, AudioDetailViewModel?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAudioQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AudioDetailViewModel?> Handle(GetAudioQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Audios
                .GetAudio(query.Id, query.PrivateKey, cancellationToken);
        }
    }
}