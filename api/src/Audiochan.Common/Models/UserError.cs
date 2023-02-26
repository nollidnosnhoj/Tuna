namespace Audiochan.Common.Models;

public interface IUserError
{
    public string Code { get; }
    public string Message { get; }
}