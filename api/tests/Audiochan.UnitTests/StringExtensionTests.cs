using System;
using Audiochan.Core.Common.Extensions;
using Xunit;

namespace Audiochan.UnitTests
{
    public class StringExtensionTests
    {
        [Theory]
        [InlineData("superuser-this-is-an-example-song", "Superuser - This is an example song")]
        [InlineData("xn-det-hr-r-en-lycklig-sng-z7bc4b", "Det här är en lycklig sång.")]
        public void ReturnValidSlugTheory(string slug, string input)
        {
            Assert.Equal(slug, input.GenerateSlug());
        }

        [Theory]
        [InlineData("happy-hardcore", "Happy Hardcore")]
        public void ReturnValidTagTheory(string tag, string input)
        {
            Assert.Equal(tag, input.GenerateTag());
        }
    }
}