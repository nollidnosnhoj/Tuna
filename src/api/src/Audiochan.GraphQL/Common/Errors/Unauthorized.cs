using Audiochan.Application.Commons.Exceptions;

namespace Audiochan.GraphQL.Common.Errors;

public class Unauthorized : IGraphQlError
{
    public string Code => GetType().Name;
    public string Message => "Unauthorized";

    public static Unauthorized? CreateErrorFrom(Exception ex)
    {
        if (ex is UnauthorizedException unauthorizedException)
            return new Unauthorized();
        return null;
    }
}