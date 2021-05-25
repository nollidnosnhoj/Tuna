using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.GetAudio
{
    public record GetAudioRequest(Guid Id) : IRequest<AudioDetailViewModel?>
    {
    }

    public class GetAudioRequestHandler : IRequestHandler<GetAudioRequest, AudioDetailViewModel?>
    {
        private readonly IAudioRepository _audioRepository;

        public GetAudioRequestHandler(IAudioRepository audioRepository)
        {
            _audioRepository = audioRepository;
        }

        public async Task<AudioDetailViewModel?> Handle(GetAudioRequest request, CancellationToken cancellationToken)
        {
            // return await _dbContext.Audios
            //     .AsNoTracking()
            //     .Include(x => x.Tags)
            //     .Include(x => x.User)
            //     .Where(x => x.Id == request.Id)
            //     .ProjectToDetail(_storageSettings)
            //     .SingleOrDefaultAsync(cancellationToken);

            return await _audioRepository.GetBySpecAsync(new GetAudioSpecification(request.Id), cancellationToken: cancellationToken);
        }
    }
}