using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Users.GetProfile
{
    public record GetProfileQuery(string Username) : IRequest<ProfileViewModel?>
    {
    }

    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ProfileViewModel?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProfileQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProfileViewModel?> Handle(GetProfileQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Users.GetProfile(query.Username, cancellationToken);
        }
    }
}