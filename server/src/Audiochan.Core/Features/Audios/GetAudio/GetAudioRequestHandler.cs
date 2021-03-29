using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.GetAudio
{
    public class GetAudioRequestHandler : IRequestHandler<GetAudioRequest, Result<AudioDetailViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly AudiochanOptions _audiochanOptions;

        public GetAudioRequestHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IOptions<AudiochanOptions> options)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _audiochanOptions = options.Value;
        }

        public async Task<Result<AudioDetailViewModel>> Handle(GetAudioRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            var audio = await _dbContext.Audios
                .DefaultSingleQueryable(request.PrivateKey, currentUserId)
                .Where(x => x.Id == request.Id)
                .ProjectToDetail(_audiochanOptions)
                .SingleOrDefaultAsync(cancellationToken);

            return audio == null
                ? Result<AudioDetailViewModel>.Fail(ResultError.NotFound)
                : Result<AudioDetailViewModel>.Success(audio);
        }
    }
}