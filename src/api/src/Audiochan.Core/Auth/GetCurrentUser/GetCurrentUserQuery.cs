using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Attributes;
using Audiochan.Core.Common.Interfaces.Persistence;
using MediatR;

namespace Audiochan.Core.Auth
{
    [Authorize]
    public record GetCurrentUserQuery(long UserId) : IRequest<CurrentUserDto?>
    {
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