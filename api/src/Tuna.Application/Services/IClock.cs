using System;

namespace Tuna.Application.Services;

public interface IClock
{
    DateTime LocalNow { get; }
    DateTime UtcNow { get; }
}