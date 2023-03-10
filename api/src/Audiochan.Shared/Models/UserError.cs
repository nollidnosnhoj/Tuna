namespace Audiochan.Shared.Models;

public interface IUserError
{
    public string Code { get; }
    public string Message { get; }
}

public class UserError : IUserError
{
    public string Code { get; }
    public string Message { get; }

    public UserError(string code, string message)
    {
        Code = code;
        Message = message;
    }
}