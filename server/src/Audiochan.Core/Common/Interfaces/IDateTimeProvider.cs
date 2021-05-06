using System;

namespace Audiochan.Core.Common.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
        DateTime FromEpochToDateTime(long epoch);
        long FromDateTimeToEpoch(DateTime dateTime);
    }
}