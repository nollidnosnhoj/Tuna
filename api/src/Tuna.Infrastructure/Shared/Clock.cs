using System;
using Tuna.Application.Services;

namespace Tuna.Infrastructure.Shared;

internal class Clock : IClock
{
    public DateTime LocalNow => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}