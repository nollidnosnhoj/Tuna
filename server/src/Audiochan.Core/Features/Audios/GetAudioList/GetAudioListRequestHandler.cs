using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Extensions.QueryableExtensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.GetAudioList
{
    public class GetAudioListRequestHandler : IRequestHandler<GetAudioListRequest, CursorList<AudioViewModel, long>>
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

        public async Task<CursorList<AudioViewModel, long>> Handle(GetAudioListRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            var audios = await _dbContext.Audios
                .BaseListQueryable(currentUserId)
                .OrderByDescending(a => a.Id)
                .ProjectToList(_storageSettings)
                .CursorPaginateAsync(request, 
                    req => req.Id, 
                    req => req.Id < request.Cursor, 
                    cancellationToken);

            var nextCursor = audios.Count < request.Size ? null : audios.LastOrDefault()?.Id;

            return new CursorList<AudioViewModel, long>(audios, nextCursor);
        }
    }
}