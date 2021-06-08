using System.Collections.Generic;

namespace Audiochan.Core.Common.Models
{
    public enum ResultError
    {
        BadRequest,
        NotFound,
        Unauthorized,
        Forbidden,
        UnprocessedEntity
    }
    
    public class Result
    {
        public Dictionary<string, string[]>? Errors { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
        public ResultError? ErrorCode { get; set; }
        
        public Result()
        {
            IsSuccess = true;
        }

        public Result(ResultError error, string message = "")
        {
            IsSuccess = false;
            ErrorCode = error;
            Message = GetDefaultMessage(error, message);
        }

        public static Result Fail(ResultError errorCode, string message = "", Dictionary<string, string[]>? errors = null)
        {
            return new(errorCode, message)
            {
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

        public static implicit operator bool(Result result) => result.IsSuccess;
        public static implicit operator Result(ResultError error) => new(error);

        protected static string GetDefaultMessage(ResultError? errorCode, string message)
        {
            if (!string.IsNullOrWhiteSpace(message)) return message;

            return errorCode switch
            {
                null => "Success",
                ResultError.NotFound => "The requested resource was not found.",
                ResultError.Unauthorized => "You are not authorized access.",
                ResultError.Forbidden => "You are authorized, but forbidden access.",
                ResultError.UnprocessedEntity => "The request payload is invalid.",
                ResultError.BadRequest => "Unable to process request.",
                _ => "An unknown error has occurred."
            };
        }
    }

    public class Result<T> : Result
    {
        public T? Data { get; private set; }

        public Result() : base()
        {
            
        }

        public Result(T data) : base()
        {
            Data = data;
        }

        public Result(ResultError error, string message = "") : base(error, message)
        {
            Data = default;
        }
        
        public new static Result<T> Fail(ResultError errorCode, string message = "", Dictionary<string, string[]>? errors = null)
        {
            return new(errorCode, message)
            {
                Errors = errors
            };
        }

        public static Result<T> Success(T data)
        {
            return new(data)
            {
                IsSuccess = true,
                Message = "Success"
            };
        }
        
        public static implicit operator bool(Result<T> result) => result.IsSuccess;
        public static implicit operator Result<T>(ResultError error) => new(error);
    }
}