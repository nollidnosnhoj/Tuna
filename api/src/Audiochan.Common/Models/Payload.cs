namespace Audiochan.Common.Models;

public abstract class Payload<TError> where TError : IUserError
{
    public IReadOnlyList<TError> Errors { get; }
    public string? Message { get; }

    protected Payload()
    {
        Errors = ArraySegment<TError>.Empty;
        Message = null;
    }

    protected Payload(params TError[] errors)
    {
        Errors = errors;
        Message = null;
    }

    protected Payload(IEnumerable<TError> errors, string? message = null)
    {
        Errors = errors.ToList();
        Message = message;
    }

    protected Payload(string? message)
    {
        Errors = ArraySegment<TError>.Empty;
        Message = message;
    }
}