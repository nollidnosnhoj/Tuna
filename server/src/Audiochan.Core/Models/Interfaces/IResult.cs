using System.Collections.Generic;
using Audiochan.Core.Enums;

namespace Audiochan.Core.Models.Interfaces
{
    public interface IResult<out TResponse>
    {
        TResponse Data { get; }
        bool IsSuccess { get; }
        ResultError? ErrorCode { get; }
        string Message { get; }
        Dictionary<string, string[]> Errors { get; }
    }
}