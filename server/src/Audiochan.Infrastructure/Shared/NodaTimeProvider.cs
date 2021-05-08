using System;
using Audiochan.Core.Common.Interfaces;
using Microsoft.IdentityModel.Tokens;
using NodaTime;

namespace Audiochan.Infrastructure.Shared
{
    public class NodaTimeProvider : IDateTimeProvider
    {
        public Instant Now => Instant.FromDateTimeUtc(DateTime.UtcNow);
    }
}