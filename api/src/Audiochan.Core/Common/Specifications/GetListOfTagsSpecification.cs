using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Common.Specifications
{
    public sealed class GetListOfTagsSpecification : Specification<Tag>
    {
        public GetListOfTagsSpecification(IEnumerable<string> tags)
        {
            Query.Where(tag => tags.Contains(tag.Name));
        }   
    }
}