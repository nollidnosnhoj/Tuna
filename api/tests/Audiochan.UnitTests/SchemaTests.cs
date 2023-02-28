using Audiochan.API.Configurations;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;

namespace Audiochan.UnitTests;

public class SchemaTests
{
    [Fact]
    public async Task PrintSchema()
    {
        var schema = await new ServiceCollection()
            .AddGraphQLServer()
            .AddAudiochanSchema()
            .BuildSchemaAsync();
        
        schema.Print().MatchSnapshot();
    }
}