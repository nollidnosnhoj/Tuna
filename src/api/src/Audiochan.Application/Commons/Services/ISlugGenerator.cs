using System.Collections.Generic;

namespace Audiochan.Application.Commons.Services
{
    public interface ISlugGenerator
    {
        /// <summary>
        /// Convert a string into a url-friendly slug.
        /// </summary>
        /// <param name="input">A string you want to convert.</param>
        /// <returns>slug</returns>
        string GenerateSlug(string input);
        
        /// <summary>
        /// Convert a list of string into a list of url-friendly slugs
        /// </summary>
        /// <param name="inputs">A list of strings you want to convert.</param>
        /// <returns>A list of slugs</returns>
        IList<string> GenerateSlugs(IEnumerable<string> inputs);
    }
}