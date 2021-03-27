using System;
using Audiochan.Core.Interfaces;

namespace Audiochan.Infrastructure.Shared
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.UtcNow;
    }
}