using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.GetRandomAudio
{
    public record GetRandomAudioQuery : IRequest<Result<AudioDetailViewModel>>
    {
    }

    public class GetRandomAudioQueryHandler : IRequestHandler<GetRandomAudioQuery, Result<AudioDetailViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly AudiochanOptions _audiochanOptions;

        public GetRandomAudioQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService,
            IOptions<AudiochanOptions> options)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _audiochanOptions = options.Value;
        }

        public async Task<Result<AudioDetailViewModel>> Handle(GetRandomAudioQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var audio = await _dbContext.Audios
                .DefaultListQueryable(currentUserId)
                .OrderBy(a => Guid.NewGuid())
                .ProjectToDetail(_audiochanOptions)
                .SingleOrDefaultAsync(cancellationToken);

            return audio == null
                ? Result<AudioDetailViewModel>.Fail(ResultError.NotFound)
                : Result<AudioDetailViewModel>.Success(audio);
        }
    }
}