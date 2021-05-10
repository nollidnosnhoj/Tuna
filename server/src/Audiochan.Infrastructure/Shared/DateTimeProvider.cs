using System;
using Audiochan.Core.Common.Interfaces;
using Microsoft.IdentityModel.Tokens;
using NodaTime;

namespace Audiochan.Infrastructure.Shared
{
    public class DateTimeProvider : IDateTimeProvider
    {
        private readonly IClock _clock;

        public DateTimeProvider(IClock clock)
        {
            _clock = clock;
        }

        public Instant Now => _clock.GetCurrentInstant();
    }
}