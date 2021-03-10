using System;
using Audiochan.Core.Interfaces;

namespace Audiochan.Infrastructure.Shared
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.UtcNow;
    }
}