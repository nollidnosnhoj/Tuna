using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Application.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Application.Features.Users.Queries.GetProfile
{
    public record GetProfileQuery(string Username) : IQueryRequest<UserDto?>
    {
    }

    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UserDto?>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetProfileQueryHandler(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<UserDto?> Handle(GetProfileQuery query, CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .Where(u => u.UserName == query.Username)
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}