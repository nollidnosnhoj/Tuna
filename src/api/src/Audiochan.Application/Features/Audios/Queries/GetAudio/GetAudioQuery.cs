using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using DistributedCacheExtensions = Audiochan.Application.Commons.Extensions.DistributedCacheExtensions;

namespace Audiochan.Application.Features.Audios.Queries.GetAudio
{
    public record GetAudioQuery(long Id) : IQueryRequest<AudioDto?>
    {
    }

    public class GetAudioQueryHandler : IRequestHandler<GetAudioQuery, AudioDto?>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetAudioQueryHandler(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<AudioDto?> Handle(GetAudioQuery query, CancellationToken cancellationToken)
        {
            return await _dbContext.Audios
                .AsNoTracking()
                .Where(a => a.Id == query.Id)
                .ProjectTo<AudioDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}