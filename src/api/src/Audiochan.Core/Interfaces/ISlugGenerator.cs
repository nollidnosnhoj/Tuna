using System.Collections.Generic;

namespace Audiochan.Core.Interfaces
{
    public interface ISlugGenerator
    {
        string GenerateSlug(string input);
        IList<string> GenerateSlugs(IEnumerable<string> inputs);
    }
}