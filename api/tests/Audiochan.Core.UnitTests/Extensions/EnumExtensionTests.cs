using Audiochan.Core.Common.Extensions;
using FluentAssertions;
using Xunit;

namespace Audiochan.Core.UnitTests.Extensions
{
    public enum TestEnum
    {
        Enum1,
        Enum2,
        Enum3,
        Enum4,
        Enum5
    }

    public class EnumExtensionTests
    {
        [Fact]
        public void CheckIfParseFalseStringIntoEnum()
        {
            const string input = "abcdef";
            var result = input.ParseToEnumOrDefault<TestEnum>();
            result.Should().Be(default(TestEnum));
        }

        [Fact]
        public void CheckIfParseTrueStringIntoEnum()
        {
            const string input = "enum5";
            var result = input.ParseToEnumOrDefault<TestEnum>();
            result.Should().Be(TestEnum.Enum5);
        }
    }
}