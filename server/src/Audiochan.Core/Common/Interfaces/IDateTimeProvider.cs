using System;
using NodaTime;

namespace Audiochan.Core.Common.Interfaces
{
    public interface IDateTimeProvider
    {
        Instant Now { get; }
    }
}