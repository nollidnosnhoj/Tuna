using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.GetAudio
{
    public record GetAudioQuery(long Id, string PrivateKey = "") : IRequest<Result<AudioDetailViewModel>>
    {
    }

    public class GetAudioQueryHandler : IRequestHandler<GetAudioQuery, Result<AudioDetailViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetAudioQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<Result<AudioDetailViewModel>> Handle(GetAudioQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            var audio = await _dbContext.Audios
                .DefaultSingleQueryable(request.PrivateKey, currentUserId)
                .Where(x => x.Id == request.Id)
                .Select(AudioMappingExtensions.AudioToDetailProjection())
                .SingleOrDefaultAsync(cancellationToken);

            return audio == null
                ? Result<AudioDetailViewModel>.Fail(ResultError.NotFound)
                : Result<AudioDetailViewModel>.Success(audio);
        }
    }
}