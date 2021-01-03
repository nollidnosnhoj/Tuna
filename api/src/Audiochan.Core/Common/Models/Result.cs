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

    public class Result : IResult
    {
        public Dictionary<string, string[]>? Errors { get; protected set; }
        public string? Message { get; protected set; }
        public bool IsSuccess { get; protected set; }
        public ResultErrorCode? ErrorCode { get; protected set; }

        public static Result Fail(ResultErrorCode errorCode, string message = null!, 
            Dictionary<string, string[]>? errors = null)
        {
            return new Result
            {
                ErrorCode = errorCode,
                Message = message,
                IsSuccess = false,
                Errors = errors
            };
        }

        public static Result Success()
        {
            return new Result
            {
                IsSuccess = true,
                Message = "Success"
            };
        }
    }
}