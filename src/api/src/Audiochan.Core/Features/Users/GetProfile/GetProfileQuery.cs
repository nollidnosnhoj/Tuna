using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Users.GetProfile
{
    public record GetProfileQuery(string Username) : IRequest<ProfileDto?>
    {
    }

    public sealed class GetProfileByUsernameSpecification : Specification<User, ProfileDto>
    {
        public GetProfileByUsernameSpecification(string username)
        {
            Query.AsNoTracking();
            Query.Where(u => u.UserName == username);
            Query.Select(UserMaps.UserToProfileFunc);
        }
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
                .GetFirstAsync(new GetProfileByUsernameSpecification(query.Username), cancellationToken);
        }
    }
}