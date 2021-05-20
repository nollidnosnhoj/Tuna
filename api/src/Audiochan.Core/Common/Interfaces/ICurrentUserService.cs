namespace Audiochan.Core.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string GetUserId();
        string GetUsername();
        bool IsAuthenticated();
    }
}