using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Mappings;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.API.Features.Audios.GetAudio
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
            return await _unitOfWork.Audios
                .AsNoTracking()
                .Include(x => x.Tags)
                .Include(x => x.User)
                .Where(x => x.Id == request.Id)
                .ProjectToDetail()
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}