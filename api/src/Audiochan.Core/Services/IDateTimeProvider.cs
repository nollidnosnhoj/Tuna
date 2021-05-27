using System;

namespace Audiochan.Core.Services
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}