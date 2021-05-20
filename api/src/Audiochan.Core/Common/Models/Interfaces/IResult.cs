using System.Collections.Generic;
using Audiochan.Core.Common.Enums;
using MediatR;

namespace Audiochan.Core.Common.Models.Interfaces
{
    public interface IResult : IResult<Unit>{}
    
    public interface IResult<out TResponse>
    {
        TResponse Data { get; }
        bool IsSuccess { get; }
        ResultError? ErrorCode { get; }
        string? Message { get; }
        Dictionary<string, string[]>? Errors { get; }
    }
}