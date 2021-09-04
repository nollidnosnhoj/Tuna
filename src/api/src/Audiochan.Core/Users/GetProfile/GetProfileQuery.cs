using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Users.GetProfile
{
    public record GetProfileQuery(string Username) : IRequest<ProfileDto?>
    {
    }

    public sealed class GetProfileByUsernameSpecification : Specification<User>
    {
        public GetProfileByUsernameSpecification(string username)
        {
            Query.AsNoTracking();
            Query.Where(u => u.UserName == username);
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
                .GetFirstAsync<ProfileDto>(new GetProfileByUsernameSpecification(query.Username), cancellationToken);
        }
    }
}