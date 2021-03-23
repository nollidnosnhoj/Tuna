using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.GetRandomAudio
{
    public record GetRandomAudioQuery : IRequest<Result<AudioDetailViewModel>>
    {
    }

    public class GetRandomAudioQueryHandler : IRequestHandler<GetRandomAudioQuery, Result<AudioDetailViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetRandomAudioQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<Result<AudioDetailViewModel>> Handle(GetRandomAudioQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var audio = await _dbContext.Audios
                .DefaultListQueryable(currentUserId)
                .OrderBy(a => Guid.NewGuid())
                .Select(AudioMappingExtensions.AudioToDetailProjection())
                .SingleOrDefaultAsync(cancellationToken);

            return audio == null
                ? Result<AudioDetailViewModel>.Fail(ResultError.NotFound)
                : Result<AudioDetailViewModel>.Success(audio);
        }
    }
}