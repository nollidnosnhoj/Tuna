namespace Audiochan.Core.Interfaces
{
    public interface ICurrentUserService
    {
        long GetUserId();
        string GetUsername();
        bool IsAuthenticated(long? userId = null);
    }
}