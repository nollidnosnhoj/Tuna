using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Extensions;
using Audiochan.Core.Extensions.MappingExtensions;
using Audiochan.Core.Extensions.QueryableExtensions;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models.Responses;
using Audiochan.Core.Models.ViewModels;
using Audiochan.Core.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Features.Audios.GetAudioList
{
    public class GetAudioListRequestHandler : IRequestHandler<GetAudioListRequest, CursorList<AudioViewModel, long?>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly MediaStorageSettings _storageSettings;

        public GetAudioListRequestHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IOptions<MediaStorageSettings> options)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _storageSettings = options.Value;
        }

        public async Task<CursorList<AudioViewModel, long?>> Handle(GetAudioListRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            return await _dbContext.Audios
                .BaseListQueryable(currentUserId)
                .OrderByDescending(a => a.Id)
                .ProjectToList(_storageSettings)
                .CursorPaginateAsync(request, cancellationToken);
        }
    }
}