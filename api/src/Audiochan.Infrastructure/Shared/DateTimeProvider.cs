using System;
using Audiochan.Core.Interfaces;

namespace Audiochan.Infrastructure.Shared
{
    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.UtcNow;
    }
}