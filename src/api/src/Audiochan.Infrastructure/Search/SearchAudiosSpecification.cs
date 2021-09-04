using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using Audiochan.Core.Audios.SearchAudios;
using Audiochan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Search
{
    public sealed class SearchAudiosSpecification : Specification<Audio>
    {
        public SearchAudiosSpecification(SearchAudiosQuery query)
        {
            var parsedTags = !string.IsNullOrWhiteSpace(query.Tags)
                ? query.Tags.Split(',')
                    .Select(t => t.Trim().ToLower())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToList()
                : new List<string>();

            Query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Q))
                Query.Where(a => 
                    EF.Functions.Like(a.Title.ToLower(), $"%{query.Q.ToLower()}%"));

            if (parsedTags.Count > 0)
                Query.Where(a => a.Tags.Any(x => parsedTags.Contains(x.Name)));
        }   
    }
}