using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.GetRandomAudio
{
    public class GetRandomAudioRequestHandler : IRequestHandler<GetRandomAudioRequest, AudioDetailViewModel>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly MediaStorageSettings _storageSettings;

        public GetRandomAudioRequestHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService,
            IOptions<MediaStorageSettings> options)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _storageSettings = options.Value;
        }

        public async Task<AudioDetailViewModel> Handle(GetRandomAudioRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            return await _dbContext.Audios
                .DefaultListQueryable(currentUserId)
                .OrderBy(a => Guid.NewGuid())
                .ProjectToDetail(_storageSettings)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}