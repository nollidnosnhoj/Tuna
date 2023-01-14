using System;
using Audiochan.Core.Services;

namespace Audiochan.Tests.Common.Mocks;

public class MockDateTimeProvider : IDateTimeProvider
{
    public MockDateTimeProvider(DateTime now)
    {
        Now = now;
    }
        
    public DateTime Now { get; }
}