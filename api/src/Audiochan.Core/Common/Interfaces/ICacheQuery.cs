using System;

namespace Audiochan.Core.Common.Interfaces
{
    public interface ICacheQuery
    {
        string CacheKey { get; }
        bool BypassCache { get; }
        TimeSpan? CacheExpiration { get; }
    }
}