using System.Collections.Generic;
using Audiochan.Core.Common.Enums;

namespace Audiochan.Core.Common.Models
{
    public interface IResult<out T> : IResult
    {
        T Data { get; }
    }
    
    public class Result<T> : Result, IResult<T>
    {
        public T Data { get; private set; } = default!;

        public new static Result<T> Fail(
            ResultErrorCode errorCode
            , string? message = null
            , Dictionary<string, string[]>? errors = null)
        {
            return new Result<T>
            {
                ErrorCode = errorCode,
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }

        public static Result<T> Success(T data)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Message = "Success",
                Data = data
            };
        }
    }
}