using System;
using Audiochan.Common.Services;
using Audiochan.Core.Services;

namespace Audiochan.Infrastructure.Shared
{
    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.UtcNow;
    }
}