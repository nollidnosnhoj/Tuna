using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.GetRandomAudio
{
    public record GetRandomAudioQuery : IRequest<Result<AudioViewModel>>
    {
    }

    public class GetRandomAudioQueryHandler : IRequestHandler<GetRandomAudioQuery, Result<AudioViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetRandomAudioQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Result<AudioViewModel>> Handle(GetRandomAudioQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var audio = await _dbContext.Audios
                .DefaultQueryable(currentUserId)
                .OrderBy(a => Guid.NewGuid())
                .ProjectTo<AudioViewModel>(_mapper.ConfigurationProvider, new {currentUserId})
                .SingleOrDefaultAsync(cancellationToken);

            return audio == null
                ? Result<AudioViewModel>.Fail(ResultError.NotFound)
                : Result<AudioViewModel>.Success(audio);
        }
    }
}