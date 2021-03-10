namespace Audiochan.Core.Interfaces
{
    public interface ICurrentUserService
    {
        string GetUserId();
        string GetUsername();
        bool IsAuthenticated();
    }
}