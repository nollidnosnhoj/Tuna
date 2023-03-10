namespace Audiochan.Shared.Models;

public interface IUserError
{
    public string Code { get; }
    public string Message { get; }
}