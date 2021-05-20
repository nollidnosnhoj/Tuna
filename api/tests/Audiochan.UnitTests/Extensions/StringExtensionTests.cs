using Audiochan.Core.Common.Extensions;
using FluentAssertions;
using Xunit;

namespace Audiochan.UnitTests.Extensions
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