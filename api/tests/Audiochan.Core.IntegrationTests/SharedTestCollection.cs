using Xunit;

namespace Audiochan.Core.IntegrationTests;

[CollectionDefinition(nameof(SharedTestCollection))]
public class SharedTestCollection : ICollectionFixture<AudiochanApiFactory>
{
    
}