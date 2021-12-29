namespace Audiochan.GraphQL.Common.Errors;

[InterfaceType("Error")]
public interface IGraphQlError
{
    public string Code { get; }
    public string Message { get; }
}

public abstract class GraphQlError : IGraphQlError
{
    public string Code => GetType().Name;
    public string Message { get; }
    
    protected GraphQlError(string message)
    {
        Message = message;
    }

    protected GraphQlError(Exception exception) : this(exception.Message)
    {
    }
}