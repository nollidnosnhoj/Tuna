using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Audios.GetAudio
{
    public record GetAudioRequest(Guid Id) : IRequest<AudioDetailViewModel?>
    {
    }

    public class GetAudioRequestHandler : IRequestHandler<GetAudioRequest, AudioDetailViewModel?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAudioRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AudioDetailViewModel?> Handle(GetAudioRequest request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Audios.GetBySpecAsync(new GetAudioSpecification(request.Id), cancellationToken: cancellationToken);
        }
    }
}