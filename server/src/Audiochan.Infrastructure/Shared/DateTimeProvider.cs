using System;
using Audiochan.Core.Common.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Audiochan.Infrastructure.Shared
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.UtcNow;
    }
}