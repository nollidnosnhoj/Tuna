using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Audiochan.Core.IntegrationTests
{
    public class TestFixtureTests : TestBase
    {
        public TestFixtureTests(TestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void ShouldNotSetDefaultDate()
        {
            var dateTimeProvider = ExecuteScope(scope => scope.GetRequiredService<IDateTimeProvider>());
            dateTimeProvider.Should().NotBeNull();
            dateTimeProvider.Now.Should().NotBe(default);
        }
    }
}