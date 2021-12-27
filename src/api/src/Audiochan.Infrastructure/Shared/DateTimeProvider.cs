using System;
using Audiochan.Application.Services;

namespace Audiochan.Infrastructure.Shared
{
    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.UtcNow;
    }
}