using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Extensions.QueryableExtensions;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Features.Audios;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Users.GetUserAudios
{
    public class GetUserAudiosRequest : IHasCursor, IRequest<CursorList<AudioViewModel>>
    {
        public string? Username { get; set; }
        public string? Cursor { get; init; }
        public int Size { get; init; }
    }

    public class GetUserAudiosRequestHandler : IRequestHandler<GetUserAudiosRequest, CursorList<AudioViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly MediaStorageSettings _storageSettings;

        public GetUserAudiosRequestHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService,
            IOptions<MediaStorageSettings> options)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _storageSettings = options.Value;
        }

        public async Task<CursorList<AudioViewModel>> Handle(GetUserAudiosRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var audios = await _dbContext.Audios
                .ExcludePrivateAudios(currentUserId)
                .Where(a => request.Username != null && a.User.UserName == request.Username.ToLower())
                .FilterUsingCursor(request.Cursor)
                .OrderByDescending(a => a.Created)
                .ThenByDescending(a => a.Id)
                .ProjectToList(_storageSettings)
                .Take(request.Size)
                .ToListAsync(cancellationToken);
            
            var lastAudio = audios.LastOrDefault();

            var nextCursor = audios.Count < request.Size
                ? null
                : lastAudio != null
                    ? CursorHelpers.EncodeCursor(lastAudio.Uploaded, lastAudio.Id.ToString())
                    : null;

            return new CursorList<AudioViewModel>(audios, nextCursor);
        }
    }
}