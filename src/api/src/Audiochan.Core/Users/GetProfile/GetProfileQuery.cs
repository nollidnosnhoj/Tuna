using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Persistence;
using MediatR;

namespace Audiochan.Core.Users
{
    public record GetProfileQuery(string Username) : IRequest<ProfileDto?>
    {
    }

    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ProfileDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProfileQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProfileDto?> Handle(GetProfileQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Users
                .GetFirstAsync<ProfileDto>(new GetUserByUsernameSpecification(query.Username), cancellationToken);
        }
    }
}