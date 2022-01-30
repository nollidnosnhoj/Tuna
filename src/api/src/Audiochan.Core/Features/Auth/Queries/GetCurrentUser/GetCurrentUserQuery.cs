using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Commons.CQRS;

using Audiochan.Core.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Auth.Queries.GetCurrentUser
{
    public record GetCurrentUserQuery(long UserId) : IQueryRequest<CurrentUserDto?>
    {
    }

    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDto?>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetCurrentUserQueryHandler(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<CurrentUserDto?> Handle(GetCurrentUserQuery query,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .Where(u => u.Id == query.UserId)
                .ProjectTo<CurrentUserDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}