using System;
using Audiochan.Core.Common.Interfaces;

namespace Audiochan.Infrastructure.Shared
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.UtcNow;
    }
}