namespace Audiochan.Application.Commons.Results;

/// <summary>
/// Represents a forbidden result
/// </summary>
public class ForbiddenErrorResult : ErrorResult
{
    public ForbiddenErrorResult() : base("Forbidden.")
    {
    }
}

public class ForbiddenErrorResult<T> : ErrorResult<T>
{
    public ForbiddenErrorResult() : base("Forbidden.")
    {
    }
}