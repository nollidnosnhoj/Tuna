using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Artists.Queries
{
    public record GetArtistFollowersQuery(string Username) : IHasOffsetPage, IRequest<OffsetPagedListDto<FollowerViewModel>>
    {
        public int Offset { get; init; }
        public int Size { get; init; }
    }
    
    public sealed class GetFollowerByTargetNameSpecification : Specification<FollowedArtist>
    {
        public GetFollowerByTargetNameSpecification(string username)
        {
            Query.AsNoTracking();
            Query.Where(u => u.Target.UserName == username);
            Query.OrderByDescending(u => u.FollowedDate);
        }
    }

    public class GetArtistFollowersQueryHandler : IRequestHandler<GetArtistFollowersQuery, OffsetPagedListDto<FollowerViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetArtistFollowersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OffsetPagedListDto<FollowerViewModel>> Handle(GetArtistFollowersQuery query,
            CancellationToken cancellationToken)
        {
            var list = await _unitOfWork.FollowedArtists
                .GetOffsetPagedListAsync<FollowerViewModel>(new GetFollowerByTargetNameSpecification(query.Username), query.Offset, query.Size, cancellationToken);
            return new OffsetPagedListDto<FollowerViewModel>(list, query.Offset, query.Size);
        }
    }
}