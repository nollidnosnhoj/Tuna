using System;

namespace Tuna.Application.Services;

public interface IDateTimeProvider
{
    DateTime LocalNow { get; }
    DateTime UtcNow { get; }
}