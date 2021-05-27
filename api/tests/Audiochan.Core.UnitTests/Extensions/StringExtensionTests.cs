using Audiochan.Core.Extensions;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.UnitTests.Extensions
{
    public class StringExtensionTests
    {
        [Theory]
        [InlineData("Happy Hardcore", "happy-hardcore")]
        public void ReturnValidTagTheory(string tag, string validInput)
        {
            var taggify = tag.GenerateSlug();
            taggify.Should().Be(validInput);
        }
    }
}