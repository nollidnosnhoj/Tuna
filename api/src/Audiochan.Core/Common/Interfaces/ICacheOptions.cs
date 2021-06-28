using System;

namespace Audiochan.Core.Common.Interfaces
{
    public interface ICacheOptions
    {
        public string Key { get; }
        public TimeSpan Expiration { get; }
    }
}