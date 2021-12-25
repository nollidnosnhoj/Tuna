using System;

namespace Audiochan.Application.Commons.Results;

/// <summary>
/// An abstract result class that can be either successful or not
/// </summary>
public abstract class Result {
    public bool Succeeded { get; protected set; }
}

/// <summary>
/// An abstract result class that can be either successful or not, that contains data.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Result<T> : Result
{
    private readonly T _data;

    public T Data =>
        Succeeded
            ? _data
            : throw new Exception("Unable to access value when result failed.");

    protected Result(T data)
    {
        _data = data;
    }

    public static implicit operator T(Result<T> result) => result._data;
    public static implicit operator Result<T>(T data) => new SuccessResult<T>(data);
}