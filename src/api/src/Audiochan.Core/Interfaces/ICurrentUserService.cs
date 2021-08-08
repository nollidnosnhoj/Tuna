using System;

namespace Audiochan.Core.Interfaces
{
    public interface ICurrentUserService
    {
        long GetUserId();
        bool TryGetUserId(out long userId);
        string GetUsername();
        bool TryGetUsername(out string username);
        bool IsAuthenticated();
    }
}