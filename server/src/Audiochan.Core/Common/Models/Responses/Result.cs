using System.Collections.Generic;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Models.Interfaces;
using MediatR;

namespace Audiochan.Core.Common.Models.Responses
{
    public record Result<TResponse> : IResult<TResponse>
    {
        public TResponse Data { get; init; } = default!;
        public Dictionary<string, string[]>? Errors { get; init; }
        public string? Message { get; init; }
        public bool IsSuccess { get; init; }
        public ResultError? ErrorCode { get; init; }

        public static Result<TResponse> Fail(ResultError errorCode, string message = "",
            Dictionary<string, string[]>? errors = null)
        {
            return new()
            {
                ErrorCode = errorCode,
                Message = GetDefaultMessage(errorCode, message),
                IsSuccess = false,
                Errors = errors
            };
        }

        public static Result<TResponse> Success(TResponse data)
        {
            return new()
            {
                IsSuccess = true,
                Data = data,
                Message = "Success"
            };
        }

        public static implicit operator bool(Result<TResponse> result)
        {
            return result.IsSuccess;
        }

        private static string GetDefaultMessage(ResultError errorCode, string message)
        {
            if (!string.IsNullOrWhiteSpace(message)) return message;

            return errorCode switch
            {
                ResultError.NotFound => "The requested resource was not found.",
                ResultError.Unauthorized => "You are not authorized access.",
                ResultError.Forbidden => "You are authorized, but forbidden access.",
                ResultError.UnprocessedEntity => "The request payload is invalid.",
                ResultError.BadRequest => "Unable to process request.",
                _ => "An unknown error has occurred."
            };
        }
    }
}