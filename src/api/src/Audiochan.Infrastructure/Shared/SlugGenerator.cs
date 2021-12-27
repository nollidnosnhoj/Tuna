using System.Collections.Generic;
using System.Linq;
using Audiochan.Application.Services;
using Slugify;

namespace Audiochan.Infrastructure.Shared
{
    public class SlugGenerator : ISlugGenerator
    {
        private readonly ISlugHelper _slugHelper;

        public SlugGenerator()
        {
            _slugHelper = new SlugHelper();
        }
        
        public string GenerateSlug(string input)
        {
            return _slugHelper.GenerateSlug(input);
        }

        public IList<string> GenerateSlugs(IEnumerable<string> inputs)
        {
            return inputs.Select(x => _slugHelper.GenerateSlug(x)).ToList();
        }
    }
}