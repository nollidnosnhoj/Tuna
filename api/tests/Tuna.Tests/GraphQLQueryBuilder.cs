using System.Text;
using GraphQL.Query.Builder;

namespace Tuna.Tests;

public class GraphQLQueryBuilder<T> : Query<T>
{
    public GraphQLQueryBuilder(string name) : base(name, new QueryOptions
    {
        Formatter = CamelCasePropertyNameFormatter.Format,
        QueryStringBuilderFactory = () => new CustomQueryStringBuilder()
    })
    {
    }
}

internal class CustomQueryStringBuilder : QueryStringBuilder
{
    public CustomQueryStringBuilder() : base(CamelCasePropertyNameFormatter.Format)
    {
    }

    protected override string FormatQueryParam(object value)
    {
        switch (value)
        {
            case Guid guidValue:
                var guidEncoded = guidValue.ToString().Replace("\"", "\\\"");
                return $"\"{guidEncoded}\"";
            case Enum enumValue:
                return ToConstantCase(enumValue.ToString());
            default:
                return base.FormatQueryParam(value);
        }
    }

    private static string ToConstantCase(string name)
    {
        StringBuilder sb = new();
        var firstUpperLetter = true;
        foreach (var c in name)
        {
            if (char.IsUpper(c))
            {
                if (!firstUpperLetter) sb.Append('_');
                firstUpperLetter = false;
            }

            sb.Append(char.ToUpperInvariant(c));
        }

        var result = sb.ToString();

        return result;
    }
}