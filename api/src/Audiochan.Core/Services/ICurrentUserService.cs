namespace Audiochan.Core.Services
{
    public interface ICurrentUserService
    {
        string GetUserId();
        bool TryGetUserId(out string userId);
        string GetUsername();
        bool TryGetUsername(out string username);
        bool IsAuthenticated();
    }
}