using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.Infrastructure.Persistence.Repositories.Abstractions;
using AutoMapper;

namespace Audiochan.Infrastructure.Persistence.Repositories;

public class ArtistRepository : EfRepository<Artist>, IArtistRepository
{
    public ArtistRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }
}