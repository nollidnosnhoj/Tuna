using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Users.GetUserAudios
{
    public class GetUserAudiosRequestHandler : IRequestHandler<GetUserAudiosRequest, PagedList<AudioViewModel>>
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

        public async Task<PagedList<AudioViewModel>> Handle(GetUserAudiosRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            return await _dbContext.Audios
                .DefaultListQueryable(currentUserId)
                .Where(a => a.User.UserName == request.Username.ToLower())
                .ProjectToList(_storageSettings)
                .PaginateAsync(request, cancellationToken);
        }
    }
}