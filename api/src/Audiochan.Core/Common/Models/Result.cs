using System.Collections.Generic;
using System.Text;
using Audiochan.Core.Common.Enums;

namespace Audiochan.Core.Common.Models
{
    public interface IResult
    {
        bool IsSuccess { get; }
        ResultStatus? ErrorCode { get; }
        string Message { get; }
        Dictionary<string, string[]> Errors { get; }

    }

    public record Result : IResult
    {
        public Dictionary<string, string[]> Errors { get; init; }
        public string Message { get; init; }
        public bool IsSuccess { get; init; }
        public ResultStatus? ErrorCode { get; init; }

        public static Result Fail(ResultStatus errorCode, string message = "", 
            Dictionary<string, string[]> errors = null)
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