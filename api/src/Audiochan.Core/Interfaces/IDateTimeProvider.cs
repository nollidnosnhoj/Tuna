using System;

namespace Audiochan.Core.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}