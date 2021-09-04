using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Auth.GetCurrentUser
{
    public record GetCurrentUserQuery(long UserId) : IRequest<CurrentUserDto?>
    {
    }

    public sealed class GetCurrentUserSpecification : Specification<User>
    {
        public GetCurrentUserSpecification(long userId)
        {
            Query.AsNoTracking();
            Query.Where(u => u.Id == userId);
        }
    }

    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDto?>
    {
        private readonly IUnitOfWork _dbContext;

        public GetCurrentUserQueryHandler(IUnitOfWork dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CurrentUserDto?> Handle(GetCurrentUserQuery query,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .GetFirstAsync<CurrentUserDto>(new GetCurrentUserSpecification(query.UserId), cancellationToken);
        }
    }
}