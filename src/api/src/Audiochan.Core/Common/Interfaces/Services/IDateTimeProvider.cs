using System;

namespace Audiochan.Core.Common.Interfaces.Services
{
    public interface IDateTimeProvider
    {
        /// <summary>
        /// The current time based on the date time provider.
        /// </summary>
        DateTime Now { get; }
    }
}