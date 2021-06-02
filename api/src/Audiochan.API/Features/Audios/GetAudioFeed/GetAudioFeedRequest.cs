using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Audios.GetAudioList;
using Audiochan.API.Mappings;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models;
using Audiochan.Infrastructure.Persistence.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.API.Features.Audios.GetAudioFeed
{
    public record GetAudioFeedRequest : IHasPage, IRequest<PagedListDto<AudioViewModel>>
    {
        public string UserId { get; init; } = string.Empty;
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class GetAudioFeedRequestHandler : IRequestHandler<GetAudioFeedRequest, PagedListDto<AudioViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAudioFeedRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedListDto<AudioViewModel>> Handle(GetAudioFeedRequest request,
            CancellationToken cancellationToken)
        {
            var followedIds = await _unitOfWork.Users
                .Include(u => u.Followings)
                .AsNoTracking()
                .Where(user => user.Id == request.UserId)
                .SelectMany(u => u.Followings.Select(f => f.TargetId))
                .ToListAsync(cancellationToken);

            return await _unitOfWork.Audios
                .AsNoTracking()
                .Include(x => x.User)
                .Where(a => a.IsPublic)
                .Where(a => followedIds.Contains(a.UserId))
                .ProjectToList()
                .OrderByDescending(a => a.Uploaded)
                .PaginateAsync(cancellationToken: cancellationToken);
        }
    }
}