using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.GetAudio
{
    public record GetAudioQuery(Guid Id) : IRequest<AudioDetailViewModel?>
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
                .AsNoTracking()
                .Include(x => x.Tags)
                .Include(x => x.User)
                .Where(x => x.Id == query.Id)
                .ProjectToDetail()
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}