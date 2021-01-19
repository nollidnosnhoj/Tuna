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
            ResultErrorCode errorCode
            , string? message = null
            , Dictionary<string, string[]>? errors = null)
        {
            return new()
            {
                ErrorCode = errorCode,
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }
        
        public new static Result<T> NotFound(string? message = null)
        {
            return Fail(ResultErrorCode.NotFound, message);
        }

        public new static Result<T> Unauthorized(string? message = null)
        {
            return Fail(ResultErrorCode.Unauthorized, message);
        }

        public new static Result<T> Forbidden(string? message = null)
        {
            return Fail(ResultErrorCode.Forbidden, message);
        }

        public new static Result<T> Invalid(string? message = null, Dictionary<string, string[]>? errors = null)
        {
            return Fail(ResultErrorCode.UnprocessedEntity, message, errors);
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