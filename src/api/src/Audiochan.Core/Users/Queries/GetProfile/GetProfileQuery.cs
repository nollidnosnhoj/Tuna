using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Persistence;
using MediatR;

namespace Audiochan.Core.Users.Queries
{
    public record GetProfileQuery(string Username) : IRequest<ArtistProfileDto?>
    {
    }

    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ArtistProfileDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProfileQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ArtistProfileDto?> Handle(GetProfileQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Artists
                .GetFirstAsync<ArtistProfileDto>(new GetUserByUsernameSpecification(query.Username), cancellationToken);
        }
    }
}