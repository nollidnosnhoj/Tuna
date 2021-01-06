using System.Collections.Generic;
using Audiochan.Core.Common.Enums;

namespace Audiochan.Core.Common.Models
{
    public interface IResult
    {
        bool IsSuccess { get; }
        ResultErrorCode? ErrorCode { get; }
        string? Message { get; }
        Dictionary<string, string[]>? Errors { get; }

    }

    public record Result : IResult
    {
        public Dictionary<string, string[]>? Errors { get; init; }
        public string? Message { get; init; }
        public bool IsSuccess { get; init; }
        public ResultErrorCode? ErrorCode { get; init; }

        public static Result Fail(ResultErrorCode errorCode, string message = null!, 
            Dictionary<string, string[]>? errors = null)
        {
            return new()
            {
                ErrorCode = errorCode,
                Message = message,
                IsSuccess = false,
                Errors = errors
            };
        }

        public static Result Success()
        {
            return new()
            {
                IsSuccess = true,
                Message = "Success"
            };
        }
    }
}