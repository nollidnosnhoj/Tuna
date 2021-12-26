using Audiochan.Application.Commons.Exceptions;

namespace Audiochan.GraphQL.Common.Errors;

public class Forbidden : GraphQlError
{
    public Forbidden(ForbiddenException exception) : base(exception.Message)
    {
    }
}