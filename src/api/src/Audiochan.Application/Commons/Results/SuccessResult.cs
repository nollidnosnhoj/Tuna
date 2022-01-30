namespace Audiochan.Application.Commons.Results;

/// <summary>
/// Represents a successful result.
/// </summary>
public class SuccessResult : Result
{
    public SuccessResult()
    {
        Succeeded = true;
    }
}

/// <summary>
/// Represents a successful result that contains data of the result.
/// </summary>
/// <typeparam name="T">The type of data.</typeparam>
public class SuccessResult<T> : Result<T>
{
    public SuccessResult(T data) : base(data)
    {
        Succeeded = true;
    }

    public static implicit operator SuccessResult(SuccessResult<T> successResult) => new();
}