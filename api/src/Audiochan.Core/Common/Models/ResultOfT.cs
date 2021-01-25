using System.Collections.Generic;
using Audiochan.Core.Common.Enums;

namespace Audiochan.Core.Common.Models
{
    public interface IResult<out T> : IResult
    {
        T Data { get; }
    }
    
    public record Result<T> : Result, IResult<T>
    {
        public T Data { get; init; } = default!;

        public new static Result<T> Fail(
            ResultStatus errorCode
            , string message = null
            , Dictionary<string, string[]> errors = null)
        {
            return new()
            {
                ErrorCode = errorCode,
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }

        public static Result<T> Success(T data)
        {
            return new()
            {
                IsSuccess = true,
                Message = "Success",
                Data = data
            };
        }
    }
}