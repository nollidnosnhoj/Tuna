using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Extensions.MappingExtensions;
using Audiochan.Core.Extensions.QueryableExtensions;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models.Settings;
using Audiochan.Core.Models.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Features.Audios.GetAudio
{
    public class GetAudioRequestHandler : IRequestHandler<GetAudioRequest, AudioDetailViewModel>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly MediaStorageSettings _storageSettings;

        public GetAudioRequestHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IOptions<MediaStorageSettings> options)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _storageSettings = options.Value;
        }

        public async Task<AudioDetailViewModel> Handle(GetAudioRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            return await _dbContext.Audios
                .BaseDetailQueryable(request.PrivateKey, currentUserId)
                .Where(x => x.Id == request.Id)
                .ProjectToDetail(_storageSettings)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}