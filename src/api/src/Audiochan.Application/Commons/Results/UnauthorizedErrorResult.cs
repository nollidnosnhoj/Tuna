namespace Audiochan.Application.Commons.Results;

/// <summary>
/// Represents a failed result because of being unauthenticated
/// </summary>
public class UnauthorizedErrorResult : ErrorResult
{
    public UnauthorizedErrorResult() : base("Unauthorized.")
    {
    }
}

/// <summary>
/// Represents a failed result because of being unauthenticated
/// </summary>
public class UnauthorizedErrorResult<T> : ErrorResult<T>
{
    public UnauthorizedErrorResult() : base("Unauthorized.")
    {
    }
}