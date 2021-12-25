using System.Collections.Generic;
using Audiochan.Application.Commons.Results;

namespace Audiochan.Application.Commons.Interfaces;

public interface IErrorResult
{
    string Message { get; }
    IReadOnlyCollection<Error> Errors { get; }
}