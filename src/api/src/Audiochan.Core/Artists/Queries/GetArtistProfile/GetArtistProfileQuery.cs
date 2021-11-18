using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Artists.Queries
{
    public record GetArtistProfileQuery(string Username) : IRequest<ArtistProfileDto?>
    {
    }
    
    public sealed class GetArtistByNameSpecification : Specification<Artist>
    {
        public GetArtistByNameSpecification(string username)
        {
            Query.AsNoTracking();
            Query.Where(u => u.UserName == username);
        }
    }

    public class GetArtistProfileQueryHandler : IRequestHandler<GetArtistProfileQuery, ArtistProfileDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetArtistProfileQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ArtistProfileDto?> Handle(GetArtistProfileQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Artists
                .GetFirstAsync<ArtistProfileDto>(new GetArtistByNameSpecification(query.Username), cancellationToken);
        }
    }
}