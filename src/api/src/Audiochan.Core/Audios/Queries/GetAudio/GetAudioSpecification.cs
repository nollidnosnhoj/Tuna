using Ardalis.Specification;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Audios.Queries
{
    public sealed class GetAudioSpecification : Specification<Audio>
    {
        public GetAudioSpecification(long id)
        {
            Query.AsNoTracking();
            Query.Where(x => x.Id == id);
        }
    }
}