using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.GetUserAudios
{
    public class GetUsersAudioQuery : IHasOffsetPage, IRequest<OffsetPagedListDto<AudioViewModel>>
    {
        public string? Username { get; set; }
        public int Offset { get; init; } = 1;
        public int Size { get; init; } = 30;
    }

    public class GetUsersAudioQueryHandler : IRequestHandler<GetUsersAudioQuery, OffsetPagedListDto<AudioViewModel>>
    {
        private readonly ApplicationDbContext _unitOfWork;
        private readonly long _currentUserId;

        public GetUsersAudioQueryHandler(ApplicationDbContext unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<OffsetPagedListDto<AudioViewModel>> Handle(GetUsersAudioQuery request,
            CancellationToken cancellationToken)
        {
            IQueryable<Audio> queryable = _unitOfWork.Users
                .AsNoTracking()
                .Include(u => u.Audios)
                .Where(u => request.Username == u.UserName.ToLower())
                .SelectMany(a => a.Audios)
                .Include(a => a.Tags)
                .Include(a => a.User);

            queryable = queryable.Where(a => a.UserId == _currentUserId || a.Visibility == Visibility.Public);

            return await queryable
                .Select(AudioMaps.AudioToView(_currentUserId))
                .OffsetPaginateAsync(request, cancellationToken);
        }
    }
}