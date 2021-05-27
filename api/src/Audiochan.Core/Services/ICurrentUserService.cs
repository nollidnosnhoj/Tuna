namespace Audiochan.Core.Services
{
    public interface ICurrentUserService
    {
        string GetUserId();
        string GetUsername();
        bool IsAuthenticated();
    }
}