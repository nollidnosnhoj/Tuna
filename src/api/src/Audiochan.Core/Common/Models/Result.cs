using System;
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
        public Dictionary<string, List<string>>? Errors { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
        public ResultError? ErrorCode { get; set; }
        
        protected Result()
        {
            IsSuccess = true;
        }

        protected Result(ResultError error, string message = "")
        {
            IsSuccess = false;
            ErrorCode = error;
            Message = GetDefaultMessage(error, message);
        }

        public static Result BadRequest(string message = "")
        {
            return new(ResultError.BadRequest, message);
        }

        public static Result Unauthorized(string message = "")
        {
            return new(ResultError.Unauthorized, message);
        }

        public static Result Forbidden(string message = "")
        {
            return new(ResultError.Forbidden, message);
        }

        public static Result NotFound(string message = "")
        {
            return new(ResultError.NotFound, message);
        }

        public static Result NotFound(Type type)
        {
            return new(ResultError.NotFound, $"{type.Name} is not found.");
        }

        public static Result NotFound<TEntity>() where TEntity : class
        {
            return new(ResultError.NotFound, $"{typeof(TEntity).Name} is not found.");
        }

        public static Result Invalid(string message = "", Dictionary<string, List<string>>? errors = null)
        {
            return new Result(ResultError.UnprocessedEntity, message)
            {
                Errors = errors
            };
        }

        public static Result Success()
        {
            return new();
        }

        public static implicit operator bool(Result result) => result.IsSuccess;
        public static implicit operator Result(ResultError error) => new(error);

        private static string GetDefaultMessage(ResultError? errorCode, string message)
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

        private Result(T data)
        {
            IsSuccess = true;
            Data = data;
        }

        private Result(ResultError error, string message = "") : base(error, message)
        {
            Data = default(T);
        }
        
        public new static Result<T> BadRequest(string message = "")
        {
            return new(ResultError.BadRequest, message);
        }

        public new static Result<T> Unauthorized(string message = "")
        {
            return new(ResultError.Unauthorized, message);
        }

        public new static Result<T> Forbidden(string message = "")
        {
            return new(ResultError.Forbidden, message);
        }

        public new static Result<T> NotFound(string message = "")
        {
            return new(ResultError.NotFound, message);
        }

        public new static Result<T> NotFound(Type type)
        {
            return new(ResultError.NotFound, $"{type.Name} is not found.");
        }

        public new static Result<T> NotFound<TEntity>() where TEntity : class
        {
            return new(ResultError.NotFound, $"{typeof(TEntity).Name} is not found.");
        }

        public new static Result<T> Invalid(string message = "", Dictionary<string, List<string>>? errors = null)
        {
            return new(ResultError.UnprocessedEntity, message)
            {
                Errors = errors
            };
        }

        public static Result<T> Success(T data)
        {
            return new(data);
        }

        public static implicit operator bool(Result<T> result) => result.IsSuccess;
        public static implicit operator Result<T>(ResultError error) => new(error);
    }
}