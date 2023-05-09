using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;
using Tuna.GraphQl.Configurations;

namespace Tuna.UnitTests;

public class SchemaTests
{
    [Fact]
    public async Task PrintSchema()
    {
        var schema = await new ServiceCollection()
            .AddGraphQLServer()
            .AddTunaSchema()
            .BuildSchemaAsync();

        schema.Print().MatchSnapshot();
    }
}