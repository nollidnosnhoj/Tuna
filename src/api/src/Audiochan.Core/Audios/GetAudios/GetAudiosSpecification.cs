using System.Linq;
using Ardalis.Specification;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Audios
{
    public sealed class GetAudiosSpecification : Specification<Audio>
    {
        public GetAudiosSpecification(GetAudiosQuery request)
        {
            Query.AsNoTracking();
            
            if (request.Tags.Count > 0)
            {
                Query.Where(a => a.Tags.Any(t => request.Tags.Contains(t)));
            }

            Query.OrderByDescending(a => a.Id);
        }
    }
}